using System.ComponentModel;

public class PasswordViewModel : INotifyPropertyChanged
{
    private string _actualPassword = string.Empty;
    private string _maskedPassword = string.Empty;

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
    private string MaskInput(string newInput, string previousMask)
    {
        if (newInput.Length > previousMask.Length)
        {
            string addedChars = newInput.Substring(previousMask.Length);
            ActualPassword += addedChars;
            return new string('*', newInput.Length);
        }
        else if (newInput.Length < previousMask.Length)
        {
            int charsToRemove = previousMask.Length - newInput.Length;
            ActualPassword = ActualPassword.Substring(0, ActualPassword.Length - charsToRemove);
            return new string('*', newInput.Length);
        }

        return previousMask;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    

}
