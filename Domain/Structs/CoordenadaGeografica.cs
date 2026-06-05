namespace FarolOrbital.Domain.Structs;

/// <summary>
/// Struct imutável que representa a localização geográfica de uma câmera ou evento.
/// Uso de struct é adequado para tipos de valor pequenos e imutáveis.
/// </summary>
public struct CoordenadaGeografica
{
    public double Latitude  { get; }
    public double Longitude { get; }
    public string Descricao { get; }

    public CoordenadaGeografica(double latitude, double longitude, string descricao = "")
    {
        if (latitude  < -90  || latitude  > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude),  "Latitude deve estar entre -90 e 90.");
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude deve estar entre -180 e 180.");

        Latitude  = latitude;
        Longitude = longitude;
        Descricao = descricao;
    }

    public override string ToString() =>
        $"({Latitude:F4}, {Longitude:F4}){(string.IsNullOrWhiteSpace(Descricao) ? "" : $" — {Descricao}")}";
}
