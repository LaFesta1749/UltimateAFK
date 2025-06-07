// File: PlayerEventHandler.cs
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace UltimateAFK
{
    public class PlayerEventHandler
    {
        private class AfkData
        {
            public float LastActivity;
            public Vector3 LastPosition;
            public Vector2 LastMouseLook;
            public bool Warned;
        }

        private readonly Dictionary<Player, AfkData> _playerActivity = new();

        public void OnVerified(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (ev.Player == null || !ev.Player.IsAlive || ev.Player.Role.Type == RoleTypeId.None || ev.Player.Role.Type == RoleTypeId.Spectator)
                    return;

                _playerActivity[ev.Player] = new AfkData
                {
                    LastActivity = Time.time,
                    LastPosition = ev.Player.Position,
                    LastMouseLook = new Vector2(ev.Player.Rotation.x, ev.Player.Rotation.y),
                    Warned = false
                };

                if (Plugin.Instance.Config.Debug)
                    Log.Debug($"[AFK] Registered player: {ev.Player.Nickname}");
            });
        }

        public void OnHurting(HurtingEventArgs ev) => Refresh(ev.Player);
        public void OnShooting(ShootingEventArgs ev) => Refresh(ev.Player);
        public void OnInteractingDoor(InteractingDoorEventArgs ev) => Refresh(ev.Player);
        public void OnInteractingElevator(InteractingElevatorEventArgs ev) => Refresh(ev.Player);
        public void OnUsingItem(UsingItemEventArgs ev) => Refresh(ev.Player);

        public void OnRoundStarted()
        {
            foreach (var player in Player.List)
            {
                if (!player.IsAlive || player.Role.Type == RoleTypeId.Spectator || player.Role.Type == RoleTypeId.None)
                    continue;

                _playerActivity[player] = new AfkData
                {
                    LastActivity = Time.time,
                    LastPosition = player.Position,
                    LastMouseLook = new Vector2(player.Rotation.x, player.Rotation.y),
                    Warned = false
                };

                if (Plugin.Instance.Config.Debug)
                    Log.Debug($"[AFK] Registered {player.Nickname} on round start.");
            }
        }

        public void OnRoleChange(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (ev.Player == null || !ev.Player.IsAlive || ev.NewRole == RoleTypeId.None || ev.NewRole == RoleTypeId.Spectator)
                    return;

                _playerActivity[ev.Player] = new AfkData
                {
                    LastActivity = Time.time,
                    LastPosition = ev.Player.Position,
                    LastMouseLook = new Vector2(ev.Player.Rotation.x, ev.Player.Rotation.y),
                    Warned = false
                };

                Log.Debug($"[AFK] Registered player on role change: {ev.Player.Nickname}");
            });
        }

        public void OnLeft(LeftEventArgs ev) => _playerActivity.Remove(ev.Player);

        public void Refresh(Player player)
        {
            if (!_playerActivity.ContainsKey(player)) return;

            _playerActivity[player].LastActivity = Time.time;
            _playerActivity[player].LastPosition = player.Position;
            _playerActivity[player].LastMouseLook = new Vector2(player.Rotation.x, player.Rotation.y);
            _playerActivity[player].Warned = false;
        }

        public IEnumerator<float> AFKCheckLoop()
        {
            while (true)
            {
                foreach (var entry in _playerActivity)
                {
                    var player = entry.Key;
                    if (player == null || !_playerActivity.ContainsKey(player)) continue;
                    var data = entry.Value;

                    if (!player.IsAlive || player.Role.Type == RoleTypeId.Spectator)
                        continue;

                    if (player.Role.Type == RoleTypeId.Scp079) // allow SCP-079 mouse movement
                    {
                        Vector2 currentLook = new(player.Rotation.x, player.Rotation.y);
                        if ((currentLook - data.LastMouseLook).sqrMagnitude > 0.01f)
                        {
                            Refresh(player);
                            continue;
                        }
                    }
                    else
                    {
                        Vector3 currentPos = player.Position;
                        Vector2 currentLook = new(player.Rotation.x, player.Rotation.y);

                        if ((currentPos - data.LastPosition).sqrMagnitude > 0.01f ||
                            (currentLook - data.LastMouseLook).sqrMagnitude > 0.01f)
                        {
                            Refresh(player);
                            continue;
                        }
                    }

                    float afkTime = Time.time - data.LastActivity;
                    var cfg = Plugin.Instance.Config;

                    if (!data.Warned && afkTime >= cfg.AFKTimeout)
                    {
                        string action = cfg.ForceSpectatorInsteadOfKick ? "spectated" : "kicked";
                        player.Broadcast(5, cfg.AFKWarningBroadcast.Replace("%action%", action));
                        data.Warned = true;
                    }
                    else if (data.Warned && afkTime >= cfg.AFKTimeout + cfg.AFKGracePeriod)
                    {
                        player.Broadcast(5, cfg.AFKFinalBroadcast);

                        if (cfg.ForceSpectatorInsteadOfKick)
                        {
                            var replacement = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Spectator);
                            if (replacement != null)
                            {
                                var originalRole = player.Role.Type;
                                var originalHP = player.Health;
                                var originalInventory = player.Items.ToList();
                                var originalPosition = player.Position;
                                var effects = player.ActiveEffects.ToList();

                                player.Role.Set(RoleTypeId.Spectator);
                                replacement.Role.Set(originalRole);

                                Timing.CallDelayed(0.5f, () =>
                                {
                                    replacement.Position = originalPosition;
                                    replacement.Health = originalHP;

                                    replacement.ClearInventory();
                                    foreach (var item in originalInventory)
                                        replacement.AddItem(item);

                                    foreach (var effect in effects)
                                        replacement.EnableEffect(effect);

                                    if (Plugin.Instance.Config.Debug)
                                        Log.Debug($"[AFK] {replacement.Nickname} replaced {player.Nickname} due to AFK.");
                                });
                            }
                            else
                            {
                                player.Role.Set(RoleTypeId.Spectator);
                                if (Plugin.Instance.Config.Debug)
                                    Log.Debug($"[AFK] No replacement found. {player.Nickname} set to Spectator.");
                            }
                        }
                        else
                        {
                            player.Kick("AFK");
                            if (Plugin.Instance.Config.Debug)
                                Log.Debug($"[AFK] {player.Nickname} was kicked for being AFK.");
                        }
                        _playerActivity.Remove(player);
                    }
                }

                yield return Timing.WaitForSeconds(5f);
            }
        }
    }
}
