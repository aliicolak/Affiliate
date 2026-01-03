namespace Application.Abstractions.Services;

/// <summary>
/// Benzersiz tracking kodu üretici
/// Single Responsibility: Sadece benzersiz kod üretimi
/// </summary>
public interface ITrackingCodeGenerator
{
    /// <summary>
    /// Yeni benzersiz tracking kodu üretir
    /// </summary>
    string Generate();
    
    /// <summary>
    /// Belirli uzunlukta kod üretir
    /// </summary>
    string Generate(int length);
}
