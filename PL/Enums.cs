/// <summary>
/// Contains collections for filtering different lists in the application.
/// Each collection represents an enumeration of a specific system type, call status, job, distance type, and call field type.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// A collection for filtering the volunteer list by system type.
    /// </summary>
    internal class SystemTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.SystemType> s_enums = (Enum.GetValues(typeof(BO.SystemType)) as IEnumerable<BO.SystemType>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the call list by call status.
    /// </summary>
    internal class StatusCallCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Statuses> s_enums = (Enum.GetValues(typeof(BO.Statuses)) as IEnumerable<BO.Statuses>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the list of available job types.
    /// </summary>
    internal class JobCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Job> s_enums = (Enum.GetValues(typeof(BO.Job)) as IEnumerable<BO.Job>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the call list by distance type.
    /// </summary>
    internal class DistanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.DistanceType> s_enums = (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the closed call list by specific closed call fields.
    /// </summary>
    internal class ClosedCallFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.ClosedCallInListField> s_enums = (Enum.GetValues(typeof(BO.ClosedCallInListField)) as IEnumerable<BO.ClosedCallInListField>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the open call list by specific open call fields.
    /// </summary>
    internal class OpenCallFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.OpenCallInListField> s_enums = (Enum.GetValues(typeof(BO.OpenCallInListField)) as IEnumerable<BO.OpenCallInListField>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the call list by common call fields.
    /// </summary>
    internal class CallInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallInListField> s_enums = (Enum.GetValues(typeof(BO.CallInListField)) as IEnumerable<BO.CallInListField>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A collection for filtering the volunteer list by volunteer field sorts.
    /// </summary>
    internal class VolunteerInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListFieldSort> s_enums = (Enum.GetValues(typeof(BO.VolunteerInListFieldSort)) as IEnumerable<BO.VolunteerInListFieldSort>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    /// <summary>
    /// A command implementation for executing actions and determining if they can be executed.
    /// </summary>
    internal class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of RelayCommand with the specified execute and canExecute logic.
        /// </summary>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute!;
        }

        /// <summary>
        /// Determines whether the command can execute.
        /// </summary>
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute(object? parameter) => _execute();

        /// <summary>
        /// Event that is triggered when the result of CanExecute changes.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
