using Core.Domain.Enums;

public sealed class Price
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    // Parameterless constructor for EF Core
    private Price() { }

    private Price(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Price amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, string currency)
    {
        return new Price(amount, (Currency)Enum.Parse(typeof(Currency), currency));
    }

    public static Price operator +(Price a, Price b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add prices in different currencies");

        return new Price(a.Amount + b.Amount, a.Currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}