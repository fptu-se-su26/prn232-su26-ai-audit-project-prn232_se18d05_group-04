using Services.Implementations;

namespace VivuCarServer.Tests;

public class DriverLicenseOcrParserTests
{
    [Fact]
    public void Parse_ExtractsVietnameseDriverLicenseFields()
    {
        const string raw = """
            GIẤY PHÉP LÁI XE / DRIVER'S LICENSE
            Số/No: 480243004966
            Họ tên/Full name: NGUYỄN MINH TUẤN
            Ngày sinh/Date of Birth: 21/06/2004
            Hạng/Class: A2
            Có giá trị đến/Expires: Không thời hạn
            """;

        var result = DriverLicenseOcrParser.Parse(raw, 0.864f, new DateTime(2026, 7, 22));

        Assert.Equal("NGUYỄN MINH TUẤN", result.FullName);
        Assert.Equal("480243004966", result.LicenseNumber);
        Assert.Equal("21/06/2004", result.DateOfBirth);
        Assert.Equal("A2", result.LicenseClass);
        Assert.Equal("Không thời hạn", result.ExpiryDate);
        Assert.Equal(86.4f, result.Confidence);
    }

    [Fact]
    public void Parse_NormalizesCommonNoiseFromRealLicenseImage()
    {
        const string raw = """
            Sô/No: 480243004966
            Ho tén/Fuil name: NGUYEN MINH TUAN
            Ngày sinhDate of Birth: 21/06/2004
            AZ
            4 trị dén£xpires: Không thời han
            """;

        var result = DriverLicenseOcrParser.Parse(raw, 0.79f, new DateTime(2026, 7, 22));

        Assert.Equal("NGUYEN MINH TUAN", result.FullName);
        Assert.Equal("480243004966", result.LicenseNumber);
        Assert.Equal("21/06/2004", result.DateOfBirth);
        Assert.Equal("A2", result.LicenseClass);
        Assert.Equal("Không thời hạn", result.ExpiryDate);
    }}
