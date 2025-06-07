[![Downloads](https://img.shields.io/github/downloads/LaFesta1749/UltimateAFK/total?label=Downloads&color=333333&style=for-the-badge)](https://github.com/LaFesta1749/UltimateAFK/releases/latest)
[![Discord](https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/PTmUuxuDXQ)

# 💤 UltimateAFK

> "Sometimes, the real enemy is... your chair."

**UltimateAFK** is an Exiled plugin designed for SCP: Secret Laboratory servers that detects players who go AFK and takes care of them so the game keeps flowing. Whether it's kicking them out or replacing them with an eager spectator – this plugin keeps your rounds alive and dynamic.

---

## 🔧 Features

* Detects inactivity based on **position** and **camera rotation**.
* Separate logic for SCP-079 (monitors only camera movement).
* Sends a configurable **warning broadcast** after timeout.
* After grace period:

  * **Kick** AFK players (default behavior), **OR**
  * Replace them with a **Spectator**, who inherits:

    * Role
    * Position
    * Health
    * Inventory
    * Active Effects
* Full config support with placeholders like `%action%` for dynamic warning messages.
* Fully supports Exiled 9.6.0.

---

## ⚙️ Config Options

```yaml
# Enable the plugin
is_enabled: true

# Enable debug logs
debug: false

# Time in seconds before a player is considered AFK
afk_timeout: 120

# Time in seconds after warning before action is taken
afk_grace_period: 30

# Should AFK players be moved to spectator instead of kicked?
force_spectator_instead_of_kick: true

# Broadcast message shown when warning the player
# %action% is replaced with "kicked" or "spectated"
afk_warning_broadcast: "<color=yellow><b>You are AFK. Move or you will be %action%!</b></color>"

# Broadcast message shown when player is removed
afk_final_broadcast: "<b><color=red>You were removed for being AFK!</color></b>"
```

---

## 🧪 How It Works

1. A player joins and their **last movement + rotation** is tracked.
2. Every few seconds the plugin checks:

   * Are they alive?
   * Did they move?
   * Did they rotate their camera?
3. If not, and the **AFK timeout** is reached:

   * Send warning broadcast.
4. If still AFK after **grace period**:

   * Kick them, **or**
   * Find a spectator and **let them take over** their life (like a cursed SCP soul swap).

---

## 🤖 Credits

* Original author: [**SrLicht**](https://github.com/SrLicht/Ultimate-AFK)
* Ported and enhanced for [**Exiled 9.6.0**](https://github.com/ExSLMod-Team/EXILED/) by **LaFesta1749**

---

## 💬 Final Words

Don't let your MTF AFK in a closet while SCPs run wild. Let UltimateAFK handle the slackers – and maybe, just maybe, give a bored spectator their time to shine.

> Your inactivity is someone else's opportunity.™
