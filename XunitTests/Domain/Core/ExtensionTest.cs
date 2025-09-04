using System.Globalization;

namespace Domain.Core;

public sealed class ExtensionTest
{
    [Theory]
    [InlineData("123", 123)]
    [InlineData("42", 42)]
    [InlineData("0", 0)]
    public void ToInteger_Should_Convert_String_To_Integer(string input, int expected)
    {
        // Act
        int result = input.ToInteger();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123, 123)]
    [InlineData(42, 42)]
    [InlineData(0, 0)]
    public void ToInteger_Should_Convert_Object_To_Integer(object input, int expected)
    {
        // Act
        int result = input.ToInteger();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToDateBr_Should_Format_Date_As_DD_MM_YYYY()
    {
        // Arrange
        DateTime date = new DateTime(2023, 10, 20);

        // Act
        string formattedDate = date.ToDateBr();

        // Assert
        Assert.Equal("20/10/2023", formattedDate);
    }

    [Theory]
    [InlineData("20/10/2023", "20/10/2023")]
    [InlineData("2023-10-20", "2023-10-20")]
    public void ToDateTime_Should_Convert_String_To_DateTime(string input, string expected)
    {
        // Arrange
        CultureInfo cultureInfo = new CultureInfo("pt-BR");
        DateTime expectedDate = DateTime.Parse(expected, cultureInfo);

        // Act
        DateTime result = input.ToDateTimeBr();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData("123,45", 123.45)]
    [InlineData("42,0", 42.0)]
    [InlineData("0,5", 0.5)]
    public void ToDecimal_Should_Convert_String_To_Decimal(string input, decimal expected)
    {
        // Act
        decimal result = input.ToDecimal();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("   1   ", true)] // com espaços
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("qualquercoisa", false)]
    public void ToBoolean_Should_Convert_String_To_Bool(string input, bool expected)
    {
        bool result = input.ToBoolean();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(42, false)]
    public void ToBoolean_Should_Convert_Int_To_Bool(int input, bool expected)
    {
        bool result = input.ToBoolean();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)1, true)]
    [InlineData((ushort)0, false)]
    [InlineData((ushort)99, false)]
    public void ToBoolean_Should_Convert_UShort_To_Bool(ushort input, bool expected)
    {
        bool result = input.ToBoolean();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBoolean_Should_Handle_Null_Object()
    {
        object input = null;
        bool result = input.ToBoolean();
        Assert.False(result);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData((ushort)1, true)]
    [InlineData((ushort)0, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(123.45, false)] // tipo não suportado
    public void ToBoolean_Should_Convert_Object_To_Bool(object input, bool expected)
    {
        bool result = input.ToBoolean();
        Assert.Equal(expected, result);
    }
}