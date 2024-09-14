using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class SmsViewModel : BaseViewModel
{
	MediaDevice device;
	private bool isSmsSupported;
	private List<string> smsFunctionalObjects;
	private string selectedSmsFunctionalObject;

	public DelegateCommand SendTextSMSCommand { get; private set; }

	public SmsViewModel() => SendTextSMSCommand = new DelegateCommand(OnSendTextSMS);

	public void Update(MediaDevice device)
	{
		this.device = device;
		IsSmsSupported = this.device?.FunctionalCategories()?.Any(c => c == FunctionalCategory.SMS) ?? false;
		SmsFunctionalObjects = this.device?.FunctionalObjects(FunctionalCategory.SMS)?.ToList();
	}

	public bool IsSmsSupported
	{
		get => isSmsSupported;
		set
		{
			isSmsSupported = value;
			NotifyPropertyChanged(nameof(IsSmsSupported));
		}
	}

	public List<string> SmsFunctionalObjects
	{
		get => smsFunctionalObjects;
		set
		{
			smsFunctionalObjects = value;
			NotifyPropertyChanged(nameof(SmsFunctionalObjects));
		}
	}

	public string SelectedSmsFunctionalObject
	{
		get => selectedSmsFunctionalObject;
		set
		{
			selectedSmsFunctionalObject = value;
			NotifyPropertyChanged(nameof(SelectedSmsFunctionalObject));
		}
	}

	public string SmsRecipient { get; set; }

	public string SmsText { get; set; }

	private void OnSendTextSMS() => device?.SendTextSMS(SelectedSmsFunctionalObject, SmsRecipient, SmsText);

}
