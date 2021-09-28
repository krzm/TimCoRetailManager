﻿using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : 
		Conductor<object>
		, IHandle<LogOnEvent>
	{
		private readonly IEventAggregator events;
		private readonly SalesViewModel salesVM;
		private readonly ILoggedInUserModel user;
		private readonly IAPIHelper helper;

		public bool IsLoggedIn
		{
			get
			{
				bool output = false;

				if (string.IsNullOrWhiteSpace(user.Token) == false)
				{
					output = true;
				}

				return output;
			}
		}

		public ShellViewModel(
			IEventAggregator events
			, SalesViewModel salesVM
			, ILoggedInUserModel user
			, IAPIHelper helper)
		{
			this.events = events;
			this.salesVM = salesVM;
			this.user = user;
			this.helper = helper;

			events.SubscribeOnPublishedThread(this);

			ActivateItemAsync(IoC.Get<LoginViewModel>()
				, new CancellationToken());
		}

		public void ExitApplication()
		{
			TryCloseAsync();
		}

		public async Task UserManagement()
		{
			await ActivateItemAsync(IoC.Get<UserDisplayViewModel>()
				, new CancellationToken());
		}

		public async Task LogOut()
		{
			user.ResetUserModel();
			helper.LogOffUser();
			await ActivateItemAsync(IoC.Get<LoginViewModel>()
				, new CancellationToken());
			NotifyOfPropertyChange(() => IsLoggedIn);
		}

		public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
		{
			await ActivateItemAsync(salesVM, cancellationToken);
			NotifyOfPropertyChange(() => IsLoggedIn);
		}
	}
}