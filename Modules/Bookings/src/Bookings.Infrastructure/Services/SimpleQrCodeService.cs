using Bookings.Application.Common.Interfaces;
using QRCoder;

namespace Bookings.Infrastructure.Services;

public class SimpleQrCodeService : IQrCodeService
{
    public byte[] GenerateQrCode(string data)
    {
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
        {
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }
    }
}
