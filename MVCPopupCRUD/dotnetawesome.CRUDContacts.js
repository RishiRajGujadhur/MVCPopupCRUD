var $dialog;

$(document).ready(function () {

    //Populate Contact
    LoadContacts();

    //Open popup
    //.on Attach an event handler function for one or more events to the selected elements.
    //events: One or more space-separated event types and optional namespaces, such as "click" or "keydown.myPlugin".
    //selector example: a.popup, A selector string to filter the descendants of the selected elements that trigger the event. If the selector is null or omitted, the event is always triggered when it reaches the selected element.
    //handler(eventObject)" type="Function":  A function to execute when the event is triggered. The value false is also allowed as a shorthand for a function that simply does return false.

    $('body').on("click", "a.popup", function (e) {
        //If this method is called, the default action of the event will not be triggered.
        //Prevent a link from opening the URL:
        e.preventDefault();
        //.attr: Set one or more attributes for the set of matched elements.
        //'href' (hyperlink) = attributeName; the name of the attribute to set.
        var page = $(this).attr('href');
        OpenPopup(page);
    });

    //Once the CountryID changes on the body
    $('body').on('change', '#CountryID', function () {      
        var countryID = $(this).val();
        //Update States
        LoadStates(countryID);
    });

    //Save Contacts once the Submit button with id #saveForm is clicked
    $("body").on('submit', '#saveForm', function (e) {
        e.preventDefault();
        SaveContacts();
    });

    //Delete Contact
    //Calling the DeleteContact function using a click event for the element with the following id '#deleteForm'
    $('body').on('submit', '#deleteForm', function (e) {
        e.preventDefault();
        DeleteContact();
    });

});


function LoadContacts() {
	//The html() method sets or returns the content (innerHTML) of the selected elements.
	//When this method is used to set content, it overwrites the content of ALL matched elements.
	$('#update_panel').html('Loading Data...');
 
	$.ajax({
        //Executing to our controller action 'GetContacts' to get the data that action returns
		url: '/home/GetContacts',
		type: 'GET',
		dataType: 'json',
		success: function (d) {
			//if byte/text length > 0
			if (d.length > 0) {
				//$data = html table element
				var $data = $('<table></table>').addClass('table table-responsive table-striped');
				//header = html table header element
				var header = "<thead><tr><th>Contact Person</th><th>Contact No</th><th>Country</th><th>State</th><th></th></tr></thead>";
				//adding html header element to the table element

				//.append() Insert content, specified by the parameter, to the end of each element 
				$data.append(header);
 
				//iterator function, which can be used to seamlessly iterate over both objects and arrays.
				$.each(d, function (i, row) {
					//Initiating the row variable with the following html columns filled with data from the database 
					//The $ sign, accepts a string containing a CSS selector which is then used to match a set of elements.
					var $row = $('<tr/>');
					$row.append($('<td/>').html(row.ContactPerson));
					$row.append($('<td/>').html(row.ContactNo));
					$row.append($('<td/>').html(row.CountryName));
					$row.append($('<td/>').html(row.StateName));
					$row.append($('<td/>').html("<a href='/home/Save/" + row.ContactID + "' class='popup'>Edit</a>&nbsp;|&nbsp;<a href='/home/Delete/" + row.ContactID + "' class='popup'>Delete</a>"));
					//Add row to data (table)
					$data.append($row);
				});
 
				$('#update_panel').html($data);
			}
			else {
				//If no data is found then update the "update-panel" div with text 'No Data Found'
				var $noData = $('<div/>').html('No Data Found!');
				$('#update_panel').html($noData);
			}
		},
		error: function () {
			alert('Error! Please try again.');
		}
	});
 
}


//open popup
function OpenPopup(Page) {
    var $pageContent = $('<div/>');
    $pageContent.load(Page);
    $dialog = $('<div class="popupWindow" style="overflow:hidden"></div>')
            .html($pageContent)
            //Setting the popupWindow div dialog attributes's values and functions
            .dialog({
                draggable: false,
                autoOpen: false,
                resizable: false,
                model: true,
                height: 500,
                width: 600,
                close: function () {
                    $dialog.dialog('destroy').remove();
                }
            })
    //The function shows a div as a dialog with a close/remove link above
    $dialog.dialog('open');
}

//Casecade dropdown - Populate states of selected country
function LoadStates(countryID) {

    //State contaings the html dropdownlist with id #StateID
    var $state = $('#StateID');
    //Select all elements that have no children (including text nodes).
    $state.empty();
    //While adding/updating the state value, show 'Please Wait...' in the dropdownlist option 
    $state.append($('<option></option>').val('').html('Please Wait...'));

    //If no country selected
    if (countryID == null || countryID == "") {
        $state.empty();
        $state.append($('<option></option>').val('').html('Select State'));
        return;
    }
    //Executing the GetStateList action method using the AJAX function
    //Perform an asynchronous HTTP (Ajax) request.
    $.ajax({
        url: '/home/GetStateList',
        type: 'GET',
        data: { 'countryID': countryID },
        dataType: 'json',
        success: function (d) {
            //Remove 'Please Wait...' value select on html Dropdownlist
            $state.empty();
            $state.append($('<option></option>').val('').html('Select State'));
            //For Each state
            $.each(d, function (i, val) {
                //Add the state name to the the dropdownlist option element
                $state.append($('<option></option>').val(val.StateID).html(val.StateName));
            });
        },
        error: function () {

        }
    });

}

//Save Contact
function SaveContacts() {
    //Validation
    if ($('#ContactPerson').val().trim() == '' ||
        $('#ContactNo').val().trim() == '' ||
        $('#CountryID').val().trim() == '' ||
        $('#StateID').val().trim() == '') {
        $('#msg').html('<div class="failed">All fields are required.</div>');
        return false;
    }
    //Initializing the contact variable with the data from the textboxes 
    var contact = {
        //If no contact id = null then return 0 else show contact value in the element of id #ContactID
        ContactID: $('#ContactID').val() == '' ? '0' : $('#ContactID').val(),
        ContactPerson: $('#ContactPerson').val().trim(),
        ContactNo: $('#ContactNo').val().trim(),
        CountryID: $('#CountryID').val().trim(),
        StateID: $('#StateID').val().trim()
    };
    //Add validation token
    contact.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    //Save Contact
    $.ajax({
        url: '/home/Save',
        type: 'POST',
        data: contact,
        dataType: 'json',
        success: function (data) {
            //alert(data.message);
            //Once save ,clear textboxes
            if (data.status) {
                $('#ContactID').val('');
                $('#ContactPerson').val('');
                $('#ContactNo').val('');
                $('#CountryID').val('');
                $('#StateID').val('');
                //Show all contacts
                LoadContacts();
                //Close create/update dialog 
                $dialog.dialog('close');
            }
        },
        error: function () {
            $('#msg').html('<div class="failed">Error! Please try again.</div>');
        }
    });
}

//Delete Contact
function DeleteContact() {
    $.ajax({
        url: '/home/delete',
        type: 'POST',
        dataType: 'json',
        //The data to be posted to the delete action in the home controller
        data: {
            //.val() : Return the value attribute (the id of the contact to be deleted)
            'id': $('#ContactID').val(),
            '__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
        },
        success: function (data) {
             
            //If deletion successful then close dialog and reload contacts
            if (data.status) {
                $dialog.dialog('close');
                LoadContacts();
            }
        },
        error: function () {
            $('#msg').html('<div class="failed">Error ! Please try again.</div>');
        }
    });
}