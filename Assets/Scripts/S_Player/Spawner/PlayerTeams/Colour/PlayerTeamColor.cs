using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerTeamColor : MonoBehaviourPunCallbacks
{
    [Header("Team Colors")]
    [SerializeField] private Color teamAColor = Color.red;
    [SerializeField] private Color teamBColor = Color.blue;

    [Header("Target Renderer")]
    [SerializeField] private Renderer targetRenderer;

    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock _propertyBlock;

    private void Start()
    {
        _propertyBlock = new MaterialPropertyBlock();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        ApplyTeamColor();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.ActorNumber == photonView.Owner.ActorNumber && changedProps.ContainsKey("Team"))
        {
            ApplyTeamColor();
        }
    }

    private void ApplyTeamColor()
    {
        if (targetRenderer == null) return;

        PlayerTeam? team = PlayerTeamAssigner.GetPlayerTeam(photonView.Owner);
        if (team == null) return;

        Color color = team == PlayerTeam.TeamA ? teamAColor : teamBColor;

        targetRenderer.GetPropertyBlock(_propertyBlock);

        if (targetRenderer.sharedMaterial.HasProperty(BaseColorProperty))
            _propertyBlock.SetColor(BaseColorProperty, color);
        else
            _propertyBlock.SetColor(ColorProperty, color);

        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
}
