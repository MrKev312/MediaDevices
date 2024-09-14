using System;
using System.Windows.Input;

namespace MediaDevicesApp.Mvvm;

public class DelegateCommand : ICommand
{
	private readonly Action execute;
	private readonly Func<bool> canExecute;

	/// <summary>
	/// Constructor
	/// </summary>
	public DelegateCommand(Action execute, Func<bool> canExecute = null)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(execute, nameof(execute));
#else
		if (execute == null)
			throw new ArgumentNullException(nameof(execute));
#endif

		this.execute = execute;
		this.canExecute = canExecute ?? new Func<bool>(() => true);
	}

	/// <summary>
	///     Method to determine if the command can be executed
	/// </summary>
	public bool CanExecute(object parameter) => canExecute();

	/// <summary>
	///     Execution of the command
	/// </summary>
	public void Execute(object parameter) => execute();

	/// <summary>
	///     Raises the CanExecuteChaged event
	/// </summary>
	public static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

	/// <summary>
	///     Protected virtual method to raise CanExecuteChanged event
	/// </summary>
	//protected virtual void OnCanExecuteChanged()
	//{
	//    CommandManagerHelper.CallWeakReferenceHandlers(this.canExecuteChangedHandlers);
	//}

	/// <summary>
	/// ICommand.CanExecuteChanged implementation
	/// </summary>
	public event EventHandler CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}
}
