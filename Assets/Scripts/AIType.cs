public enum AIType
{
    PlayerControlled,
    ClosestEnemy,
    ThreatScoring,   // Weighted scoring (distance, speed, etc.)
    ErdemsSpecial,
    None           // No movement / idle
}