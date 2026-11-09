namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// The amount a <see cref="WithholdingDefinition"/> applies its rate to.
/// </summary>
/// <remarks>
/// The minimum base is always compared against the subtotal of the document (the value before taxes); only the amount the
/// rate multiplies changes. In Colombia the income and ICA withholdings are computed on the subtotal and the VAT
/// withholding on the VAT of the lines that form its base (DUR 1625 arts. 1.3.2.1.12 and 1.3.2.1.13).
/// </remarks>
public enum WithholdingBasis
{
    /// <summary>
    /// The rate applies to the subtotal: the value of the lines before taxes.
    /// </summary>
    Subtotal = 1,

    /// <summary>
    /// The rate applies to the tax charged on the lines that form the base.
    /// </summary>
    Tax = 2
}
