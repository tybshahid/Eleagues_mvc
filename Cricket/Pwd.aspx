<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pwd.aspx.cs" Inherits="Cricket.Utilities.Pwd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Eleagues - Password Setup</title>
    <link rel="shortcut icon" href="~/favicon.ico" type="image/x-icon" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="panel panel-danger">
            <div class="panel-heading">
                <h3 class="panel-title">Password Manager
                </h3>
            </div>
            <div class="panel-body">
                <div class="form-group form-group-sm">
                    <label class="col-sm-2 control-label">
                        Player
                    </label>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlPlayer" class="form-control" runat="server"></asp:DropDownList>
                    </div>
                    <label class="col-sm-2 control-label">
                        User Name
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtUserName" class="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group form-group-sm">
                    <label class="col-sm-2 control-label">
                        Password
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtPassword" class="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group form-group-sm">
                    <label class="col-sm-2 control-label">
                        Admin Password
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtAdminPassword" class="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div id="divSetPassword" runat="server" class="form-group form-group-sm">
                    <div class="col-sm-12">
                        <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" class="btn btn-danger btn-flat btn-block" runat="server" Text="Set Password" />
                    </div>
                </div>
                <div id="divPasswordSet" runat="server" visible="false" class="form-group form-group-sm">
                    <div class="col-sm-12">
                        <span class="btn btn-success btn-flat btn-block disabled">Password has been updated</span>
                    </div>
                </div>
            </div>
        </div>
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title">Generate an Encrypted Password
                </h3>
            </div>
            <div class="panel-body">
                <div class="form-group form-group-sm">
                    <label class="col-sm-2 control-label">
                        Password
                    </label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtPlainPwd" class="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group form-group-sm">
                    <label class="col-sm-2 control-label">
                        Password Hash
                    </label>
                    <div class="col-sm-10">
                        <asp:Label ID="lblPwdHash" class="form-control" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="form-group form-group-sm">
                    <div class="col-sm-12">
                        <asp:Button ID="btnGenerate" OnClick="btnGenerate_Click" class="btn btn-primary btn-flat btn-block" runat="server" Text="Generate" />
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
