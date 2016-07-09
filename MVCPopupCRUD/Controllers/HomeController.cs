using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPopupCRUD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetContacts()
        {
            List<Contact> all = null;

            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                
                //the following linq expression which sends data from the dc variable (database) to the contacts variable
                var contacts = (from a in dc.Contacts
                                
                                join b in dc.Countries on a.CountryID equals b.CountryID
                                join c in dc.States on a.StateID equals c.StateID
                                select new
                                {
                                    //a contains the persons data from the database
                                    a,
                                    b.CountryName,
                                    c.StateName
                                });
                //If the variable contacts is successfully populated using above code then
                if (contacts != null)
                {   
                    all = new List<Contact>();
                    foreach (var i in contacts)
                    {
                        //Create a new contact object and populate it with persons data as shown below
                        Contact con = i.a;
                        con.CountryName = i.CountryName;
                        con.StateName = i.StateName;
                        //Add con the list of con on each iterations (each row/person) 
                        all.Add(con);
                    }
                }
            }
            //Data: A value that indicates whether HTTP GET requests from the client are allowed.
            //JsonRequestBehavior: Gets or sets a value that indicates whether HTTP GET requests from the client are allowed.
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Fetch Country from database
        private List<Country> GetCountry()
        {
            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                return dc.Countries.OrderBy(a => a.CountryName).ToList();
            }
        }

        //Fetch State from database
        private List<State> GetState(int countryID)
        {
            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                return dc.States.Where(a => a.CountryID.Equals(countryID)).OrderBy(a => a.StateName).ToList();
            }
        }

        //return states as json data
        public JsonResult GetStateList(int countryID)
        {
            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                return new JsonResult { Data = GetState(countryID), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //Get contact by ID
        public Contact GetContact(int contactID)
        {
            Contact contact = null;
            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         join c in dc.States on a.StateID equals c.StateID
                         where a.ContactID.Equals(contactID) //
                         select new
                         {
                             a,
                             b.CountryName,
                             c.StateName
                         }).FirstOrDefault(); //Returns the first element of a sequence, or a default value if the sequence contains no elements.
                //     contains no elements.
                if (v != null)// if the database contains a contact
                {
                    contact = v.a;
                    contact.CountryName = v.CountryName;
                    contact.StateName = v.StateName;
                }
                return contact;
            }
        }

        //Action for save/add and edit
        public ActionResult Save(int id = 0)
        {
            List<Country> Country = GetCountry();
            List<State> States = new List<State>();

            if (id > 0)
            {
                var c = GetContact(id);
                if (c != null)
                {
                    ViewBag.Countries = new SelectList(Country, "CountryID", "CountryName", c.CountryID);
                    ViewBag.States = new SelectList(GetState(c.CountryID), "StateID", "StateName", c.StateID);
                }
                else
                {
                    return HttpNotFound(); //Returns an instance of the System.Web.Mvc.HttpNotFoundResult class.
                }
                return PartialView("Save", c); //Creates a System.Web.Mvc.PartialViewResult object that renders a partial view, by using the specified view name and model.
            }
            else // if no contact found
            {
                ViewBag.Countries = new SelectList(Country, "CountryID", "CountryName");
                ViewBag.States = new SelectList(States, "StateID", "StateName");
                return PartialView("Save");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Contact c)
        {
            string message = "";
            bool status = false;

            #region
            //ModelState.IsValid tells you if any model errors have been added to ModelState. The ModelState represents the submitted values and errors in said values during a POST. 
            //The validation process respects the attributes like Required and EmailAddress, and we can add custom errors to the validation if we so desire. ValidationSummary and ValidationMessageFor read directly from ModelState to display errors to the user.
            //ModelState has two purposes: to store the value submitted to the server, and to store the validation errors associated with those values.
            //IsValid: Gets a value that indicates whether this instance of the model-state dictionary
            //     is valid.
            //Returns:
            //     true if this instance is valid; otherwise, false.
            #endregion
            if (ModelState.IsValid)
            {
                using (myDatabaseEntities dc = new myDatabaseEntities())
                {
                    //If there is a contact in the database then update it as follows
                    if (c.ContactID > 0)
                    {
                        //.Equals(): Returns
                        //     true if obj has the same value as this instance; otherwise, false.

                        //FirstOrDefault(): Returns the first element of a sequence, or a default value if the sequence
                        //     contains no elements.

                        //We are providing Contact c as an argument to the Post method Save 
                        //v = Contact c if it exists in the database a 
                        var v = dc.Contacts.Where(a => a.ContactID.Equals(c.ContactID)).FirstOrDefault();
                        //create contact if does not already exists, 
                        //collect data in variable v using argument provided c.
                        if (v != null)
                        {
                            v.ContactPerson = c.ContactPerson;
                            v.ContactNo = c.ContactNo;
                            v.CountryID = c.CountryID;
                            v.StateID = c.StateID;
                        }
                        else
                        {
                            return HttpNotFound();
                        }
                    }
                    else
                    {
                        // add new contact to the database 
                        dc.Contacts.Add(c);
                    }
                    //Saves all changes made in this context to the underlying database.
                    dc.SaveChanges();
                    status = true;
                    message = "Successfully Saved.";
                }
            }
            else
            {
                message = "Error! Please try again.";
            }

            //Data returned : status of the Save (failure or success) 
            //message : return the variable message 's value we previously initialized
            return new JsonResult { Data = new { status = status, message = message } };
        }

        public ActionResult Delete(int id)
        {
            var c = GetContact(id);
            //if there is no contact in the database
            if (c == null)
            {
                return HttpNotFound();
            }
            //Return contact to be deleted
            return PartialView(c);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken] : Represents an attribute that is used to prevent forgery (faking) of a request.
        [ValidateAntiForgeryToken]
        //[ActionName(...)] Represents an attribute that is used for the name of an action.
        [ActionName("Delete")]
        public ActionResult DeleteContact(int id)
        {
            bool status = false;
            string message = "";
            using (myDatabaseEntities dc = new myDatabaseEntities())
            {
                //Get contact to be deleted
                var v = dc.Contacts.Where(a => a.ContactID.Equals(id)).FirstOrDefault();
                //If contact to be deleted exists in the database (dc)
                if (v != null)
                {
                    
                    // .Remove(...) : Removes the given collection of entities from the context underlying the
                    //     set with each entity being put into the Deleted state such that it will be
                    //     deleted from the database when SaveChanges is called.
                    
                    //Remove the contact from the database
                    dc.Contacts.Remove(v);
                    
                    //Refresh the database
                    // Saves all changes made in this context to the underlying database.
                    dc.SaveChanges();
                    status = true;
                    message = "Successfully Deleted!";
                }
                else
                {
                    return HttpNotFound();
                }
            }
            //Return status of the deletion and message "Successfully Deleted!"
            return new JsonResult { Data = new { status = status, message = message } };
        }
    }
}
