using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PlayerTeamAssigner
{
    private const string TEAM_KEY = "Team";

    public static void AssignTeamToPlayer(Player player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Log.Warning("[TeamAssigner] Solo el MasterClient puede asignar equipos.");
            return;
        }

        if (HasTeamAssigned(player))
        {
            Log.Info($"[TeamAssigner] {player.NickName} ya tiene equipo: {GetPlayerTeam(player)}");
            return;
        }

        int teamACount = 0;
        int teamBCount = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == player.ActorNumber) continue;

            PlayerTeam? team = GetPlayerTeam(p);
            if (team == PlayerTeam.TeamA) teamACount++;
            else if (team == PlayerTeam.TeamB) teamBCount++;
        }

        PlayerTeam assignedTeam = teamACount <= teamBCount ? PlayerTeam.TeamA : PlayerTeam.TeamB;

        Hashtable props = new Hashtable { { TEAM_KEY, (int)assignedTeam } };
        player.SetCustomProperties(props);

        Log.Info($"[TeamAssigner] {player.NickName} asignado a {assignedTeam} (A:{teamACount}, B:{teamBCount})");
    }

    public static void AssignTeamsToAllPlayers()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AssignTeamToPlayer(player);
        }
    }

    public static PlayerTeam? GetPlayerTeam(Player player)
    {
        if (player.CustomProperties.TryGetValue(TEAM_KEY, out object teamObj))
        {
            return (PlayerTeam)(int)teamObj;
        }
        return null;
    }

    public static PlayerTeam? GetLocalPlayerTeam()
    {
        return GetPlayerTeam(PhotonNetwork.LocalPlayer);
    }

    public static bool HasTeamAssigned(Player player)
    {
        return player.CustomProperties.ContainsKey(TEAM_KEY);
    }
}
