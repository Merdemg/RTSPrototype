public enum AIType
{
    PlayerControlled,
    ClosestEnemy,
    ThreatScoring,   // Weighted scoring (distance, speed, etc.)
    None           // No movement / idle
}