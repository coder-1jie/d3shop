﻿using Microsoft.AspNetCore.Components.Forms;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.Permission;

namespace NetCorePal.D3Shop.Web.Admin.Client.Components;

public partial class AddRole
{
    [Inject] private IRolesService RolesService { get; set; } = default!;
    [Inject] private IPermissionsService PermissionsService { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;

    [Parameter] public EventCallback OnItemAdded { get; set; }

    private List<Permission> _allPermissions = [];
    private bool _modalVisible;
    private bool _modalConfirmLoading;
    private Form<CreateRoleRequest> _form = default!;
    private Tabs _tabs = default!;
    private string[] _treeCheckedKeys = [];


    private CreateRoleRequest _newRoleModel = new();

    private async Task ShowModal()
    {
        _modalVisible = true;
        _allPermissions = await GetAllPermissions();
    }

    private async Task<List<Permission>> GetAllPermissions()
    {
        var response = await PermissionsService.GetAll();
        if (response.Success) return response.Data.ToList();
        _ = Message.Error(response.Message);
        return [];
    }

    private void CloseModal()
    {
        _modalVisible = false;
        _newRoleModel = new CreateRoleRequest();
        _treeCheckedKeys = [];
        _tabs.GoTo(0);
    }

    private async Task Form_OnFinish(EditContext editContext)
    {
        _modalConfirmLoading = true;
        StateHasChanged();
        var response = await RolesService.CreateRole(_newRoleModel);
        if (response.Success)
        {
            _ = Message.Success("创建成功！");
            CloseModal();
            await OnItemAdded.InvokeAsync();
        }
        else
        {
            _ = Message.Error(response.Message);
        }

        _modalConfirmLoading = false;
    }

    private void Form_OnFinishFailed(EditContext editContext)
    {
        _tabs.GoTo(0);
    }

    private void Tree_OnCheck(TreeEventArgs<string> e)
    {
        _newRoleModel.PermissionCodes = _allPermissions
            .Where(p => _treeCheckedKeys.Contains(p.Code))
            .Select(p => p.Code)
            .ToList();
    }
}