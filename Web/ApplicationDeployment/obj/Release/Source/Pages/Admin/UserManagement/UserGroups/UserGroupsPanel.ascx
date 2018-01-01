<%--  License

// Copyright (c) 2013, MatrixPACS Inc.
// All rights reserved.
// http://www.MatrixPACS.ca
//
// This file is part of the MatrixPACS RIS/PACS open source project.
//
// The MatrixPACS RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The MatrixPACS RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the MatrixPACS RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

--%>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGroupsPanel.ascx.cs" Inherits="MatrixPACS.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel" %>

<%@ Register Src="UserGroupsGridPanel.ascx" TagName="UserGroupsGridPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Table ID="Table1" runat="server">
                <asp:TableRow>
					<asp:TableCell Wrap="false">
						<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
							<div class="SearchPanelParentDiv">
								<div class="SearchPanelChildDiv">
									<asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, GroupName %>"
									           CssClass="SearchTextBoxLabel"/><br />
									<ccUI:TextBox ID="GroupName" runat="server" CssClass="SearchTextBox"/>
								</div>
								<div class="SearchPanelChildDiv">
									<asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
										<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>"
											OnClick="SearchButton_Click" />
									</asp:Panel>
								</div>
							</div>
						</asp:Panel>
					</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td>
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddUserGroupButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="AddUserGroupButton_Click"/>
                                        <ccUI:ToolbarButton ID="EditUserGroupButton" runat="server" SkinID="<%$Image:EditButton%>" OnClick="EditUserGroupButton_Click"/>
                                        <ccUI:ToolbarButton ID="DeleteUserGroupButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="DeleteUserGroupButton_Click"/>
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server"   CssClass="SearchPanelResultContainer">
                            <table class="SearchPanelResultContainerTable">
                                 <tr><td><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td class="SearchPanelResultContainerTd"><localAsp:UserGroupsGridPanel ID="UserGroupsGridPanel" runat="server" height="500px" /></td></tr>
                            </table>
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    </ContentTemplate>
</asp:UpdatePanel>