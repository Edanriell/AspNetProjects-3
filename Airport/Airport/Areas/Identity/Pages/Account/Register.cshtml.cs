﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Airport.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

namespace Airport.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
	private readonly IEmailSender _emailSender;
	private readonly IUserEmailStore<IdentityUser> _emailStore;
	private readonly ILogger<RegisterModel> _logger;
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IUserStore<IdentityUser> _userStore;

	public RegisterModel(
		UserManager<IdentityUser> userManager,
		IUserStore<IdentityUser> userStore,
		SignInManager<IdentityUser> signInManager,
		ILogger<RegisterModel> logger,
		IEmailSender emailSender)
	{
		_userManager = userManager;
		_userStore = userStore;
		_emailStore = GetEmailStore();
		_signInManager = signInManager;
		_logger = logger;
		_emailSender = emailSender;
	}

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
	public InputModel Input { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

	public SelectListItem[] FrequentFlyerClasses { get; } =
		{
			new() { Text = "Gold", Value = "Gold" },
			new() { Text = "Silver", Value = "Silver" },
			new() { Text = "Bronze", Value = "Bronze" }
		};


	public async Task OnGetAsync(string returnUrl = null)
	{
		ReturnUrl = returnUrl;
		ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
	}

	public async Task<IActionResult> OnPostAsync(string returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");
		ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		if (ModelState.IsValid)
		{
			var user = CreateUser();

			await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
			await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
			var result = await _userManager.CreateAsync(user, Input.Password);

			if (result.Succeeded)
			{
				await AddClaims(user);

				_logger.LogInformation("User created a new account with password.");

				var userId = await _userManager.GetUserIdAsync(user);
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					null,
					new { area = "Identity", userId, code, returnUrl },
					Request.Scheme);

				await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

				if (_userManager.Options.SignIn.RequireConfirmedAccount)
					return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });

				await _signInManager.SignInAsync(user, false);
				return LocalRedirect(returnUrl);
			}

			foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
		}

		// If we got this far, something failed, redisplay form
		return Page();
	}

	private IdentityUser CreateUser()
	{
		try
		{
			return Activator.CreateInstance<IdentityUser>();
		}
		catch
		{
			throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
												$"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
												$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
		}
	}

	private IUserEmailStore<IdentityUser> GetEmailStore()
	{
		if (!_userManager.SupportsUserEmail)
			throw new NotSupportedException("The default UI requires a user store with email support.");
		return (IUserEmailStore<IdentityUser>)_userStore;
	}

	private async Task AddClaims(IdentityUser user)
	{
		if (Input.DateOfBirth.HasValue)
		{
			var newClaim = new Claim(ClaimTypes.DateOfBirth, Input.DateOfBirth.Value.ToString("yyyy-MM-dd"),
				ClaimValueTypes.Date);
			await _userManager.AddClaimAsync(user, newClaim);
		}

		if (!string.IsNullOrEmpty(Input.BoardingPassNumber))
		{
			var newClaim = new Claim(Claims.BoardingPassNumber, Input.BoardingPassNumber);
			await _userManager.AddClaimAsync(user, newClaim);
		}

		if (!string.IsNullOrEmpty(Input.EmployeeNumber))
		{
			var newClaim = new Claim(Claims.EmployeeNumber, Input.EmployeeNumber);
			await _userManager.AddClaimAsync(user, newClaim);
		}

		if (!string.IsNullOrEmpty(Input.FrequentFlyerClass))
		{
			var newClaim = new Claim(Claims.FrequentFlyerClass, Input.FrequentFlyerClass);
			await _userManager.AddClaimAsync(user, newClaim);
		}

		if (Input.IsBannedFromLounge)
		{
			var newClaim = new Claim(Claims.IsBannedFromLounge, "true");
			await _userManager.AddClaimAsync(user, newClaim);
		}
	}

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
	{
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
			MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Display(Name = "Employee Number")] public string EmployeeNumber { get; set; }

		[Display(Name = "Boarding Pass Number")]
		public string BoardingPassNumber { get; set; }

		[Display(Name = "Date of Birth")]
		[DataType(DataType.Date)]
		public DateTime? DateOfBirth { get; set; }

		[Display(Name = "Is banned from Lounge?")]
		public bool IsBannedFromLounge { get; set; }

		[Display(Name = "FrequentFlyerClass")] public string FrequentFlyerClass { get; set; }
	}
}