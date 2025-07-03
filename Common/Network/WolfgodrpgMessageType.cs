namespace Wolfgodrpg.Common.Network
{
    /// <summary>
    /// Enum centralizado para todos os tipos de mensagens de rede do mod.
    /// </summary>
    public enum WolfgodrpgMessageType : byte
    {
        SyncRPGPlayer,
        SyncHunger,
        SyncSanity,
        SyncStamina,
        SyncClass,
        SyncClassLevel,
        SyncExperience,
        UnlockAbility,
        UpdateVitals,
        SyncDash
    }
} 