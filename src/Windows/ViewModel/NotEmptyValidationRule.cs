using System.Globalization;
using System.Windows.Controls;

namespace BirthdayNotifications.Windows.ViewModel {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  public class NotEmptyValidationRule : ValidationRule {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      return string.IsNullOrWhiteSpace((value ?? "").ToString())
          ? new ValidationResult(false, "Field is required.")
          : ValidationResult.ValidResult;
    }
  }
}
