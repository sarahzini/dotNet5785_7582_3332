/// <summary>
/// This class contains various value converters for controlling visibility in the UI based on specific conditions.
/// Each converter modifies the visibility or a property value based on call statuses, assignments, or other business logic.
/// </summary>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using BO;

namespace PL
{
    /// <summary>
    /// Converts call status to visibility. Displays when the status is "Open" or "OpenToRisk" with no assignments.
    /// </summary>
    public class StatusOpenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.CallInList c = (BO.CallInList)value;
            if (c.TotalAssignment == 0 && (c.Status == BO.Statuses.Open || c.Status == BO.Statuses.OpenToRisk))
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts call status to visibility. Displays when the status is "InAction" or "InActionToRisk".
    /// </summary>
    public class StatusInActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.CallInList c = (BO.CallInList)value;
            if (c.Status == BO.Statuses.InActionToRisk || c.Status == BO.Statuses.InAction)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts assignment value to visibility. Displays when the assignment is not zero.
    /// </summary>
    public class AssignmentVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != 0)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Controls visibility of the delete button based on volunteer status (completed, canceled, expired calls).
    /// </summary>
    public class VisibilityDeleteButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.VolunteerInList v = (BO.VolunteerInList)value;
            if (v.CompletedCalls == 0 && v.CanceledCalls == 0 && v.ExpiredCalls == 0 && v.ActualCallId == null)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts status value to a boolean for visibility purposes (true or false).
    /// </summary>
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((BO.Statuses)value)
                {
                    case BO.Statuses.Expired:
                    case BO.Statuses.Closed:
                    case BO.Statuses.InAction:
                    case BO.Statuses.InActionToRisk:
                        return false;
                    case BO.Statuses.Open:
                    case BO.Statuses.OpenToRisk:
                    case BO.Statuses.All:
                        return true;
                    default:
                        return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts status value to visibility. Displays when status is open, open to risk, or all.
    /// </summary>
    public class StatusOpenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((BO.Statuses)value)
                {
                    case BO.Statuses.Expired:
                    case BO.Statuses.Closed:
                        return false;
                    case BO.Statuses.InAction:
                    case BO.Statuses.InActionToRisk:
                    case BO.Statuses.Open:
                    case BO.Statuses.OpenToRisk:
                    case BO.Statuses.All:
                        return true;
                    default:
                        return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts null values to visibility (hidden if null).
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts boolean values to visibility (true makes visible, false makes hidden).
    /// </summary>
    public class TrueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Controls visibility based on the button text ("Update").
    /// </summary>
    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Update")
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Converts "Update" string value to true.
    /// </summary>
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == "Update";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Converts "Add" string value to true.
    /// </summary>
    public class ConvertAddToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == "Add";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Converts password string to a masked format (using asterisks).
    /// </summary>
    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string password)
            {
                return new string('*', password.Length);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("One-way binding only.");
        }
    }

    /// <summary>
    /// Converts password to a masked format with asterisks for UI display.
    /// </summary>
    public class PasswordMaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return new string('*', value.ToString()!.Length);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
