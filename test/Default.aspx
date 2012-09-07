<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="test._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" rel="stylesheet" href="css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView ID="GridView1" runat="server"
     AllowPaging="True"   
            AllowSorting="True"
            AutoGenerateColumns="False"
            DataKeyNames="LinkID"
            GridLines="None"
            CssClass="mGrid"
            PagerStyle-CssClass="pgr"
            AlternatingRowStyle-CssClass="alt"
            Width="50%" HorizontalAlign="Center"
            >  
            <Columns>  
                    <asp:ImageField DataImageUrlField="Picture"  />  
                    <asp:BoundField DataField="Title" HeaderText="Title" ReadOnly="True"   />  
                    <asp:HyperLinkField DataNavigateUrlFields="Url" HeaderText="Url"    Text="link"/> 
                    <asp:BoundField DataField="OwnerComment" HeaderText="Comment" ReadOnly="True"   />  
                    <asp:BoundField DataField="CreatedTime" HeaderText="CreatedTime" ReadOnly="True"   /> 
                    <asp:BoundField DataField="Summary" HeaderText="Summary" ReadOnly="True"   />  
            </Columns>  
        </asp:GridView>  
    </div>
    </form>
</body>
</html>
