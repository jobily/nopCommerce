﻿using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ShoppingCartController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IPermissionService _permissionService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    #endregion

    #region Ctor

    public ShoppingCartController(ICustomerService customerService,
        IPermissionService permissionService,
        IShoppingCartService shoppingCartService,
        IShoppingCartModelFactory shoppingCartModelFactory)
    {
        _customerService = customerService;
        _permissionService = permissionService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> CurrentCarts()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
            return AccessDeniedView();

        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartSearchModelAsync(new ShoppingCartSearchModel());

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
            return await AccessDeniedJsonAsync();

        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetCartDetails(ShoppingCartItemSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
            return await AccessDeniedJsonAsync();

        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(searchModel.CustomerId)
                       ?? throw new ArgumentException("No customer found with the specified id");

        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartItemListModelAsync(searchModel, customer);

        return Json(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> DeleteItem(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
            return await AccessDeniedJsonAsync();

        await _shoppingCartService.DeleteShoppingCartItemAsync(id);

        return new NullJsonResult();
    }

    #endregion
}