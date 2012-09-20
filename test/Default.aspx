<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
Inherits="test._Default" %>
  <!DOCTYPE html>
  <html lang="en">
    
    <head id="Head1" runat="server">
      <meta charset="utf-8">
      <title>Link Browsing</title>
      <link type="text/css" rel="stylesheet" href="css/style.css"
      />
      <script type="text/javascript" src="http://cdn.jquerytools.org/1.2.7/full/jquery.tools.min.js"></script>
      <script type="text/javascript">
          $(document).ready(function () {
              $(".tooltipable").each(function () {
                  $(this).tooltip({
                      tip: "#" + $(this).children(".tooltip").first().attr("id"),
                      position: 'bottom center',
                      offset: [1, 0]
                  });
              });
          });
      </script>
    </head>
    
    <body>
      <form id="form1" runat="server">
        <header>
           <h1>Link Browsing</h1>
           <h2>browse your facebook links today!</h2>
        </header>
        <section class="center">
            <header style="width:500px;margin:auto;">
                <asp:Label id="Label1" runat="server" AssociatedControlID="searchBox">Search:</asp:Label>
                <asp:TextBox ID="searchBox" runat="server" Width="100px"></asp:TextBox>
                <div class="ddlist-div">
                    <asp:DropDownList runat="server" ID="searchDDL" CssClass="ddlist">
                        <asp:ListItem Value="URL">URL</asp:ListItem>
                        <asp:ListItem Value="Title">Title</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button CssClass="search" ID="Button1" runat="server" Text="Go!" OnClick="SerachUrlClick" />
                <asp:Button CssClass="search" ID="Button2" runat="server" Text="All" OnClick="AllClicked"/>
            </header>
            <article>
                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" GridLines="None" CssClass="mGrid" PagerStyle-CssClass="pgr"
                RowStyle-CssClass="alt" OnRowDataBound="RowDataBounded">
                  <Columns>
                    <asp:ImageField DataImageUrlField="Picture" ControlStyle-Width="120px" ControlStyle-Height="80px" />
                    <asp:HyperLinkField DataNavigateUrlFields="Url" HeaderText="Title" DataTextField="Title"
                    />
                    <asp:BoundField ItemStyle-CssClass="textCenter" DataField="OwnerComment" HeaderText="Comment" ReadOnly="True"
                    />
                    <asp:BoundField DataField="CreatedTime" HeaderText="CreatedTime" ReadOnly="True"
                    />
                    <asp:BoundField DataField="Summary" HeaderText="Summary" ReadOnly="True"
                    ItemStyle-Width="25%" />
                    <asp:TemplateField HeaderText="Likes" ItemStyle-CssClass="tooltipable"
                    ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                      <ItemTemplate>
                        <asp:Image ID="Image1" ImageUrl="~/css/like.jpeg" runat="server" Width="15px" />
                        <asp:Label runat="server" ID="likes"></asp:Label>
                        <div class="tooltip" id="tooltip" runat="server">abc</div>
                      </ItemTemplate>
                    </asp:TemplateField>
                  </Columns>
                </asp:GridView>
            </article>
        </section>
        <footer class="textCenter"> <a href="mailto:amit.yogev90@gmail.com">Amit Yogev</a>
        </footer>
      </form>
    </body>
  </html>