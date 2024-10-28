// Usage
//   const num = 1234567.89;
//   const formattedNumber = NumberFormatter.formatNumber(num, 2); // "1,234,567.89"
//   const formattedCurrency = NumberFormatter.formatCurrency(num, 'USD'); // "$1,234,567.89"

export class NumberFormatter {
  static formatNumber(value: number, format: number): string {
    return new Intl.NumberFormat('en-US', { style: 'decimal', minimumFractionDigits: format }).format(value);
  }

  static formatCurrency(value: number, currencyCode: string): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: currencyCode }).format(value);
  }
}

