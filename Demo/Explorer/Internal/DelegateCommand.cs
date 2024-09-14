using System;
using System.Windows.Input;

namespace ExplorerCtrl.Internal;

/// <summary>
/// This class allows delegating the commanding logic to methods passed as parameters,
/// and enables a View to bind commands to objects that are not part of the element tree.
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
internal sealed class DelegateCommand(Action execute, Func<bool> canExecute = null) : ICommand
{
	private readonly Action execute = execute ?? throw new ArgumentNullException(nameof(execute));
	private readonly Func<bool> canExecute = canExecute ?? new Func<bool>(() => true);

	/// <summary>
	/// Method to determine if the command can be executed
	/// </summary>
	public bool CanExecute(object param) => canExecute();

	/// <summary>
	/// Execution of the command
	/// </summary>
	public void Execute(object param) => execute();

	/// <summary>
	/// ICommand.CanExecuteChanged implementation
	/// </summary>
	public event EventHandler CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}
}

/// <summary>
/// This class allows delegating the commanding logic to methods passed as parameters,
/// and enables a View to bind commands to objects that are not part of the element tree.
/// </summary>
/// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
internal sealed class DelegateCommand<T> : ICommand
{
	private readonly Action<T> execute;
	private readonly Func<T, bool> canExecute;

	/// <summary>
	/// Constructor
	/// </summary>
	public DelegateCommand(Action<T> execute)
	{
		this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
		canExecute ??= new Func<T, bool>(t => true);
	}

	/// <summary>
	/// Method to determine if the command can be executed
	/// </summary>
	public bool CanExecute(object parameter) => canExecute((T)parameter);

	/// <summary>
	/// Execution of the command
	/// </summary>
	public void Execute(object parameter) => execute((T)parameter);

	/// <summary>
	/// ICommand.CanExecuteChanged implementation
	/// </summary>
	public event EventHandler CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}
}
