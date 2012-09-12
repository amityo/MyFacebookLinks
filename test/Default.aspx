<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="test._Default" %>

<!DOCTYPE html>
<html lang="en">
    <head runat="server">
        <meta charset="utf-8">  
        <title>Link Browsing</title>
        <link type="text/css" rel="stylesheet" href="css/style.css" />
    </head>
    <body>
        <header>
            <h1>Link Browsing</h1>
            <h2>browse your facebook links today!</h2>
        </header>
        <aside id="leftSide">
        </aside>
        <section class="center">
            <article>
            <form id="form1" runat="server">
            <asp:GridView ID="GridView1" runat="server"
             AllowPaging="True"   
                    AllowSorting="True"
                    AutoGenerateColumns="False"
                    GridLines="None"
                    CssClass="mGrid"
                    PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt"
                    Width="50%" HorizontalAlign="Center"
                    >  
                    <Columns>  
                            <asp:ImageField DataImageUrlField="Picture"  />  
                            <asp:HyperLinkField DataNavigateUrlFields="Url" HeaderText="Title"  DataTextField="Title"/> 
                            <asp:BoundField DataField="OwnerComment" HeaderText="Comment" ReadOnly="True"   />  
                            <asp:BoundField DataField="CreatedTime" HeaderText="CreatedTime" ReadOnly="True"   /> 
                            <asp:BoundField DataField="Summary" HeaderText="Summary" ReadOnly="True"/>  
                    </Columns>  
                </asp:GridView>  
            </form>
            </article>
        </section>
        <footer class="textCenter">
        <a href="mailto:amit.yogev90@gmail.com" >Amit Yogev</a>
        </footer>
    </body>
</html>
