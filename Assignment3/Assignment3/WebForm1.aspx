<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Assignment3.WebForm1"  Theme="Theme1"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

<%--    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="Scripts/jquery.dynDateTime.min.js" type="text/javascript"></script>
<script src="Scripts/calendar-en.min.js" type="text/javascript"></script>
    <link href="App_Themes/Theme1/calendar-blue.css" rel="stylesheet" />

    <script type="text/javascript" src="jQuery-Date-Time-Picke-Plugin-Simple-Datetimepicker/jquery.simple-dtpicker.js"></script>
    <link type="text/css" href="jQuery-Date-Time-Picke-Plugin-Simple-Datetimepicker/jquery.simple-dtpicker.css" rel="stylesheet" />--%>

</head>
<body>
    
    <form id="form1" runat="server">
        <div class="wrapper">
    <h1 class="h1">Truck Delivery Order Form</h1>
    <p>Please enter your order details below. Fields marked with a '*' are required</p><br />
        <%--<script type="text/javascript">
            

            $(document).ready(function () {
                $("#<%=txtDelDate.ClientID %>").dynDateTime({

            showsTime: true,
            ifFormat: "%Y/%m/%d %H:%M",
            daFormat: "%l;%M %p, %e %m,  %Y",
            align: "BR",
            electric: false,
            singleClick: false,
            displayArea: ".siblings('.dtcDisplayArea')",
            
            button: ".next()"
        });
    });
</script>--%>
    <div>
        <asp:Label id="lblMemID" Text="Membership ID" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtMemID" runat="server" CssClass="TextBoxes" /><br /><br /> 
        <asp:Button id="btnMem" runat="server" OnClick="btnMem_Click" Text="Find Member" />
        <asp:Label ID="lblMemOutput" runat="server" />
        <asp:Label id="lblFName" Text="*First name" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtFName" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqFName" Text="Required" runat="server" ControlToValidate="txtFName" ForeColor="Red" Display="Dynamic" CssClass="Errors" />--%><br /> <br />
        <asp:Label id="lblLName" Text="*Last name" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtLName" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqLName" Text="Required" runat="server" ControlToValidate="txtLName" ForeColor="Red" Display="Dynamic" CssClass="Errors" /><br /><br />--%>
        <asp:Label id="lblDOB" Text="DOB" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtDOB" runat="server" CssClass="TextBoxes" /><br /><br/>
        <asp:Label id="lblOriginAdd" Text="*Origin Delivery Address" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtOriginAdd" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqOAdd" Text="Required" runat="server" ControlToValidate="txtOriginAdd" ForeColor="Red" Display="Dynamic" CssClass="Errors"/><br /><br />--%>
        <asp:Label id="lblOriginCode" Text="*Origin Postcode" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtOriginCode" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqOCode" Text="Required" runat="server" ControlToValidate="txtOriginCode" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%>
        <asp:CustomValidator ID="cusOCode" Text="Not a valid postcode" runat="server" ControlToValidate="txtOriginCode" OnServerValidate="cusDate_ServerValidate" ForeColor="Red" Display="Dynamic" CssClass="Errors" /><br /><br />
        <asp:Label id="lblDstAdd" Text="*Destination Delivery Address" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtDstAdd" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqDstAdd" Text="Required" runat="server" ControlToValidate="txtDstAdd" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%><br /><br />
        <asp:Label id="lblDstCode" Text="*Destination Postcode" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtDstCode" runat="server" CssClass="TextBoxes" />
        <%--<asp:RequiredFieldValidator ID="reqDstCode" Text="Required" runat="server" ControlToValidate="txtDstCode" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%>
        <asp:CustomValidator ID="cusDCode" Text="Not a valid postcode" runat="server" ControlToValidate="txtDstCode" OnServerValidate="cusDate_ServerValidate" ForeColor="Red" Display="Dynamic" CssClass="Errors"/><br /><br />
        <asp:Label id="lblBAdd" Text="*Billing Address" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtBAdd" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqBAdd" Text="Required" runat="server" ControlToValidate="txtBAdd" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%><br /><br />
        <asp:Label id="lblPhone" Text="*Phone Number" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtPhone" runat="server" CssClass="TextBoxes" />
        <%--<asp:RequiredFieldValidator ID="reqPhone" Text="Required" runat="server" ControlToValidate="txtPhone" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%>
        <%--<asp:RegularExpressionValidator ID="regPhone" Text="Not a phone number" runat="server" ControlToValidate="txtPhone"  ForeColor="Red" Display="Dynamic"
            ValidationExpression="0[0-8]\d{8}" CssClass="Errors"/>--%> <br /><br />
        <asp:Label id="lblDelDate" Text="*Delivery Date(dd/mm/yyyy)" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtDelDate" runat="server" CssClass="TextBoxes" />
        <br /><br />
        <asp:Label id="lblDelTime" Text="*Delivery Time" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtDelTimeHours" runat="server" CssClass="TextBoxes" Width="50" />
        &nbsp;&nbsp;
        <asp:TextBox id="txtDelTimeMinutes" runat="server" CssClass="TextBoxes" Width="50" />
        <%--<asp:RequiredFieldValidator ID="reqDelTimeHours" Text="Required" runat="server" ControlToValidate="txtDelTimeHours" ForeColor="Red" Display="Dynamic" />--%>
        <%--<asp:RequiredFieldValidator ID="reqDelTimeMinutes" Text="Required" runat="server" ControlToValidate="txtDelTimeMinutes" ForeColor="Red" Display="Dynamic" />--%>
        <%--<asp:RangeValidator ID="ranHours" Text="Must be between 0 and 23" Type="Integer" runat="server"
            ControlToValidate="txtDelTimeHours" ForeColor="Red" MinimumValue="0" MaximumValue="23" Display="Dynamic" />--%>
        <%--<asp:RangeValidator ID="ranMin" Text="Must be between 0 and 59" Type="Integer" runat="server"
            ControlToValidate="txtDelTimeMinutes" ForeColor="Red" MinimumValue="0" MaximumValue="59" Display="Dynamic" />--%>
        <asp:Label id="msgError" Text="" runat="server" Visible="false" ForeColor="Red" CssClass="Errors" /> <br /><br />
        <asp:Label id="lblTrucks" Text="*Trucks to hire" runat="server" CssClass="Labels" />
        <asp:TextBox id="txtTrucks" runat="server" CssClass="TextBoxes" /> 
        <%--<asp:RequiredFieldValidator ID="reqTrucks" Text="Required" runat="server" ControlToValidate="txtTrucks" ForeColor="Red" Display="Dynamic" CssClass="Errors"/>--%><br /><br />
        <asp:Label ID="lblOutput" Text="" runat="server" /><br /><br />
        <asp:Label ID="lblPrice" Text="" runat="server" /><br />
         <asp:Label ID="lblTestOutput" Text="TestLabel" runat="server" /><br />
        <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click"/>
        
    </div>
        </div>
    </form>
</body>
</html>
