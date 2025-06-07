// File: Plugin.cs
using Exiled.API.Features;
using MEC;
using System;
using Player = Exiled.API.Features.Player;
using PlayerHandlers = Exiled.Events.Handlers.Player;
using ServerHandlers = Exiled.Events.Handlers.Server;

namespace UltimateAFK
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; } = null!;

        public override string Author => "SrLicht (ported and enhanced by LaFesta1749)";
        public override string Name => "UltimateAFK";
        public override Version Version => new(2, 1, 0);
        public override Version RequiredExiledVersion => new(9, 6, 0);

        private PlayerEventHandler _handler = null!;

        public override void OnEnabled()
        {
            Instance = this;
            _handler = new PlayerEventHandler();

            PlayerHandlers.Hurting += _handler.OnHurting;
            PlayerHandlers.Shooting += _handler.OnShooting;
            PlayerHandlers.InteractingDoor += _handler.OnInteractingDoor;
            PlayerHandlers.InteractingElevator += _handler.OnInteractingElevator;
            PlayerHandlers.UsingItem += _handler.OnUsingItem;
            PlayerHandlers.ChangingRole += _handler.OnRoleChange;
            PlayerHandlers.Left += _handler.OnLeft;
            PlayerHandlers.Verified += _handler.OnVerified;
            ServerHandlers.RoundStarted += _handler.OnRoundStarted;

            Timing.CallDelayed(1f, () => Timing.RunCoroutine(_handler.AFKCheckLoop()));

            Log.Info($"{Name} v{Version} by {Author} has been enabled!");
        }

        public override void OnDisabled()
        {
            PlayerHandlers.Hurting -= _handler.OnHurting;
            PlayerHandlers.Shooting -= _handler.OnShooting;
            PlayerHandlers.InteractingDoor -= _handler.OnInteractingDoor;
            PlayerHandlers.InteractingElevator -= _handler.OnInteractingElevator;
            PlayerHandlers.UsingItem -= _handler.OnUsingItem;
            PlayerHandlers.ChangingRole -= _handler.OnRoleChange;
            PlayerHandlers.Left -= _handler.OnLeft;
            PlayerHandlers.Verified -= _handler.OnVerified;
            ServerHandlers.RoundStarted -= _handler.OnRoundStarted;

            Instance = null!;
            _handler = null!;
        }
    }
}
