using System.ComponentModel;

public class PasswordViewModel : INotifyPropertyChanged
{
    // Stores the actual password entered by the user
    private string _actualPassword = string.Empty;

    // Stores the masked version of the password (e.g., "****")
    private string _maskedPassword = string.Empty;

    // Event to notify UI when a property changes
    public event PropertyChangedEventHandler? PropertyChanged;

    // Method to trigger the PropertyChanged event
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Property that returns the masked password
    public string MaskedPassword
    {
        get => _maskedPassword;
        set
        {
            if (_maskedPassword != value)
            {
                _maskedPassword = MaskInput(value, _maskedPassword);
                OnPropertyChanged(nameof(MaskedPassword));
            }
        }
    }

    // Property that stores the actual password but is privately set
    public string ActualPassword
    {
        get => _actualPassword;
        private set
        {
            if (_actualPassword != value)
            {
                _actualPassword = value;
                OnPropertyChanged(nameof(ActualPassword));
            }
        }
    }

    // Method to update the masked password while keeping track of the actual password
    private string MaskInput(string newInput, string previousMask)
    {
        // Case when the new input is longer (characters added)
        if (newInput.Length > previousMask.Length)
        {
            string addedChars = newInput.Substring(previousMask.Length);
            ActualPassword += addedChars;
            return new string('*', newInput.Length);
        }
        // Case when the new input is shorter (characters removed)
        else if (newInput.Length < previousMask.Length)
        {
            int charsToRemove = previousMask.Length - newInput.Length;
            ActualPassword = ActualPassword.Substring(0, ActualPassword.Length - charsToRemove);
            return new string('*', newInput.Length);
        }

        // Return previous mask if no changes are detected
        return previousMask;
    }
}
