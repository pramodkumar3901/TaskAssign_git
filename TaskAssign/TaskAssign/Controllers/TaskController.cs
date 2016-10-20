using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskAssign.Controllers
{

    public class TaskController : Controller
    {
        //
        // GET: /Task/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Task/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Task/Create
        [Authorize]
        public ActionResult Create()
        {
            var db = new TaskAssign.Models.DatabaseContext();

            /// All dropdownlist populating 
            var result = db.Lookups.Where(x => x.LookupType == "Task_Type" || x.LookupType == "Priority").ToList();
            var taskTypeList = new SelectList(result.Where(x => x.LookupType == "Task_Type").ToList(), "LookupId", "LookupName");
            var priorityList = new SelectList(result.Where(x => x.LookupType == "Priority").ToList(), "LookupId", "LookupName");
            var MembersList = new SelectList(db.Members.ToList(), "UserId", "Name");

            ViewBag.taskTypeList = taskTypeList;
            ViewBag.priorityList = priorityList;
            ViewBag.MembersList = MembersList;
            ///
            ViewBag.RoleId = Convert.ToInt32(Session["Role"].ToString());
            if (Convert.ToInt32(Session["Role"].ToString()) > 2)
            {
                ViewBag.MemberId = Convert.ToInt32(Session["UserId"].ToString());
                getNotification(Convert.ToInt32(Session["UserId"].ToString()));
            }
            else
            {
                ViewBag.MemberId = 0;
                getNotification(0);
            }

            return View();
        }

        //
        // POST: /Task/Create

        [HttpPost]
        [Authorize]
        public ActionResult SaveTask(TaskAssign.Models.TaskList TL)
        {

            try
            {
                var db = new TaskAssign.Models.DatabaseContext();

                var toInsert = new Models.Task
                {
                    TaskType = TL.TaskType,
                    Comments = TL.Comments,
                    Description = TL.Description,
                    Title = TL.Title,
                    PostedDate = DateTime.Now,
                    PostedTime = DateTime.Now.TimeOfDay,
                    PostedBy = Convert.ToInt32(Session["UserId"].ToString()),
                    ModuleName = TL.ModuleName,
                    Priority = TL.Priority,
                    InitStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId,
                    DevStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId
                };
                db.Tasks.Add(toInsert);
                db.SaveChanges();

                int taskId = toInsert.TaskId;

                saveHistory(taskId, "Posted", "");

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Task/Edit/5

        public ActionResult _Edit(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();

            /// All dropdownlist populating 
            var result = db.Lookups.Where(x => x.LookupType == "Task_Type" || x.LookupType == "Priority").ToList();
            var taskTypeList = new SelectList(result.Where(x => x.LookupType == "Task_Type").ToList(), "LookupId", "LookupName");
            var priorityList = new SelectList(result.Where(x => x.LookupType == "Priority").ToList(), "LookupId", "LookupName");
            var MembersList = new SelectList(db.Members.ToList(), "UserId", "Name");

            ViewBag.taskTypeList = taskTypeList;
            ViewBag.priorityList = priorityList;
            ViewBag.MembersList = MembersList;
            ///

            TaskAssign.Models.TaskList taskDetails = (from task in db.Tasks
                                                      where task.TaskId == id
                                                      select new TaskAssign.Models.TaskList
                                                      {
                                                          TaskId = task.TaskId,
                                                          TaskType = task.TaskType,
                                                          Comments = task.Comments,
                                                          Description = task.Description,
                                                          Title = task.Title,
                                                          PostedDate = task.PostedDate,
                                                          PostedTime = task.PostedTime,
                                                          PostedBy = task.PostedBy,
                                                          ModuleName = task.ModuleName,
                                                          Priority = task.Priority,
                                                          DevStatus = task.DevStatus,
                                                          InitStatus = task.InitStatus
                                                      }).SingleOrDefault();



            return PartialView(taskDetails);
        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        public ActionResult _Edit(int id, TaskAssign.Models.TaskList taskLists)
        {
            try
            {
                var db = new TaskAssign.Models.DatabaseContext();
                var tastToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

                tastToUpdate.TaskType = taskLists.TaskType;
                tastToUpdate.TaskType = taskLists.TaskType;
                tastToUpdate.Comments = taskLists.Comments;
                tastToUpdate.Description = taskLists.Description;
                tastToUpdate.Title = taskLists.Title;
                tastToUpdate.PostedDate = DateTime.Now.Date;
                tastToUpdate.PostedTime = DateTime.Now.TimeOfDay;
                tastToUpdate.PostedBy = Convert.ToInt32(Session["UserId"].ToString());
                tastToUpdate.ModuleName = taskLists.ModuleName;
                tastToUpdate.Priority = taskLists.Priority;
                tastToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                tastToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

                db.SaveChanges();

                saveHistory(id, "Edited", "");

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Task/Delete/5

        public ActionResult Delete(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToDelete = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            db.Tasks.Remove(taskToDelete);
            db.SaveChanges();

            return RedirectToAction("Create");
        }

        public ActionResult Close(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToClose = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToClose.InitStatus = db.Lookups.Where(x => x.LookupName == "Closed" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
            taskToClose.TesterStatus = db.Lookups.Where(x => x.LookupName == "Closed" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;

            db.SaveChanges();
            return RedirectToAction("Create");
        }

        //
        // POST: /Task/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult _AssignTasks(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();

            var AllMembersList = new SelectList(db.Members.ToList(), "UserId", "Name");

            TaskAssign.Models.TaskList result = (from task in db.Tasks
                                                 where task.TaskId == id
                                                 select new TaskAssign.Models.TaskList
                                                 {
                                                     TaskId = id,
                                                     AssignedBy = task.AssignedBy,
                                                     AllottedHrs = task.AllottedHrs,
                                                     AssignedTo = task.AssignedTo
                                                 }).SingleOrDefault();


            ViewBag.AllMembersList = AllMembersList;

            return PartialView(result);
        }

        [HttpPost]
        public ActionResult _AssignTasks(int id, TaskAssign.Models.TaskList AssignTask)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToUpdate.AssignedTo = AssignTask.AssignedTo;
            taskToUpdate.AllottedHrs = AssignTask.AllottedHrs;
            taskToUpdate.AssignedBy = Convert.ToInt32(Session["UserId"].ToString());
            taskToUpdate.AssignedOn = DateTime.Now;

            if (AssignTask.SctrictMode == false)
            {

                taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

                db.SaveChanges();

                string assingedToName = db.Members.Where(x => x.UserId == AssignTask.AssignedTo).SingleOrDefault().Name;
                saveHistory(id, "Assigned", assingedToName + "[#" + AssignTask.AssignedTo + "] for " + AssignTask.AllottedHrs + " Hours");
            }
            else
            {
                taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

                if (taskToUpdate.AllottedHrs > 0)
                {
                    taskToUpdate.DueDate = taskToUpdate.AssignedOn.AddHours(taskToUpdate.AllottedHrs);
                }
                taskToUpdate.StartedDate = taskToUpdate.AssignedOn;

                db.SaveChanges();

                string assingedToName = db.Members.Where(x => x.UserId == AssignTask.AssignedTo).SingleOrDefault().Name;
                saveHistory(id, "Assigned-s", assingedToName + "[#" + AssignTask.AssignedTo + "] for " + AssignTask.AllottedHrs + " Hours with due date: " + taskToUpdate.DueDate);
            }



            return RedirectToAction("Create");
        }


        public ActionResult _RejectWithReason(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();


            TaskAssign.Models.TaskList result = (from task in db.Tasks
                                                 where task.TaskId == id
                                                 select new TaskAssign.Models.TaskList
                                                 {
                                                     RejectReason = task.RejectReason
                                                 }).SingleOrDefault();


            return PartialView(result);
        }

        [HttpPost]
        public ActionResult _RejectWithReason(int id, TaskAssign.Models.TaskList rejectTask)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
            taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Rejected" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

            int userId = Convert.ToInt32(Session["UserId"].ToString());
            string rejectedBy = db.Members.Where(x => x.UserId == userId).SingleOrDefault().Name;

            string reasonMsg = "<div class='replyHeader'>" +
                "Rejected On:" + DateTime.Now.ToString() + "<br/>" +
                "Rejected By:" + rejectedBy + "<br/>" +
                "----------------------<br/>" +
                rejectTask.RejectReason + "<br />" +
                "----------------------<br/>" +
                "</div>";
            taskToUpdate.RejectReason = "<div style='color:gray !important;'>" + taskToUpdate.RejectReason + "</div>" + reasonMsg;
            taskToUpdate.AssignedTo = Convert.ToInt32(Session["UserId"].ToString());

            db.SaveChanges();

            saveHistory(id, "Rejected", rejectTask.RejectReason);

            return RedirectToAction("Create");
        }

        [Authorize]
        public ActionResult _ListTasks(string type, int memberId)
        {
            ViewBag.UserId = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.Role = Convert.ToInt16(Session["Role"].ToString());

            var db = new TaskAssign.Models.DatabaseContext();

            var listOfAllTasks = new List<Models.Task>();
            int userId = Convert.ToInt32(Session["UserId"].ToString());

            var lookupCache = db.Lookups.Where(x => x.LookupType == "Init_Status" || x.LookupType == "Dev_Status" || x.LookupType == "Test_Status").ToList();

                if (memberId == 0)
                {
                    if (type == "Pool")
                    {
                        int lookupId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.InitStatus == lookupId).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Assign")
                    {
                        int initId = lookupCache.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        int devId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.InitStatus == initId && x.DevStatus==devId).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Working")
                    {
                        int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.RequestFlag == false).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Rejected")
                    {
                        int devId = lookupCache.Where(x => x.LookupName == "Rejected" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus== initId).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Reopened")
                    {
                        int initId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        int devId = lookupCache.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        int testId = lookupCache.Where(x => x.LookupName == "Re-Open" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId && x.DevStatus==devId ).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Closed")
                    {
                        int initId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        int testId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Requested")
                    {

                        int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.InitStatus == initId && x.DevStatus == devId && x.RequestFlag == true).OrderByDescending(x => x.Priority).ToList();
                    }
                    else if (type == "Completed")
                    {
                        int  devId= lookupCache.Where(x => x.LookupName == "Completed" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                        int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                        listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId).OrderByDescending(x => x.Priority).ToList();
                    }
                }
                ///IF TASKS OF PARTICULAR MEMBERS ARE SELECTED
                else
                {
                    /// if admin or project manager was logged in
                    if (ViewBag.role != 4)
                    {
                        if (type == "Pool")
                        {
                            int lookupId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == lookupId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Assign")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == initId && x.DevStatus == devId && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Working")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.RequestFlag == false && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Rejected")
                        {
                            int devId = lookupCache.Where(x => x.LookupName == "Rejected" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Reopened")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int testId = lookupCache.Where(x => x.LookupName == "Re-Open" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId && x.DevStatus == devId && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Closed")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int testId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Requested")
                        {

                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == initId && x.DevStatus == devId && x.RequestFlag == true && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Completed")
                        {
                            int devId = lookupCache.Where(x => x.LookupName == "Completed" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.AssignedTo == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                    }
                    else 
                    {
                        if (type == "Pool")
                        {
                            int lookupId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == lookupId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Posted")
                        {
                            int lookupId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == lookupId && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Working")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.RequestFlag == false && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Rejected")
                        {
                            int devId = lookupCache.Where(x => x.LookupName == "Rejected" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Reopened")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int testId = lookupCache.Where(x => x.LookupName == "Re-Open" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId && x.DevStatus == devId && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Closed")
                        {
                            int initId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int testId = lookupCache.Where(x => x.LookupName == "Closed" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.TesterStatus == testId && x.InitStatus == initId && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Requested")
                        {

                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            int devId = lookupCache.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.InitStatus == initId && x.DevStatus == devId && x.RequestFlag == true && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                        else if (type == "Completed")
                        {
                            int devId = lookupCache.Where(x => x.LookupName == "Completed" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
                            int initId = lookupCache.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
                            listOfAllTasks = db.Tasks.Where(x => x.DevStatus == devId && x.InitStatus == initId && x.PostedBy == memberId).OrderByDescending(x => x.Priority).ToList();
                        }
                    }
                }
          
            List<TaskAssign.Models.TaskList> taskLists = new List<Models.TaskList>();
            foreach (var TL in listOfAllTasks)
            {
                string assignToName = "";
                string testerStatusName = "";
                var lookups = db.Lookups.ToList();
                if (TL.AssignedTo > 0)
                {
                    assignToName = db.Members.Where(x => x.UserId == TL.AssignedTo).SingleOrDefault().Name;
                }
                if (TL.TesterStatus > 0)
                {
                    testerStatusName = lookups.Where(x => x.LookupId == TL.TesterStatus).SingleOrDefault().LookupName;
                }

                taskLists.Add(new Models.TaskList
                {
                    TaskId = TL.TaskId,
                    TaskType = TL.TaskType,
                    TaskTypeName = lookups.Where(x => x.LookupId == TL.TaskType).SingleOrDefault().LookupName,
                    Comments = TL.Comments,
                    Description = TL.Description,
                    Title = TL.Title,
                    PostedDate = TL.PostedDate,
                    PostedTime = TL.PostedTime,
                    PostedBy = TL.PostedBy,
                    PostedByName = db.Members.Where(x => x.UserId == TL.PostedBy).SingleOrDefault().Name,
                    ModuleName = TL.ModuleName,
                    Priority = TL.Priority,
                    PriorityName = lookups.Where(x => x.LookupId == TL.Priority).SingleOrDefault().LookupName,
                    InitStatus = TL.InitStatus,
                    InitStausName = lookups.Where(x => x.LookupId == TL.InitStatus).SingleOrDefault().LookupName,
                    DevStatus = TL.DevStatus,
                    DevStausName = lookups.Where(x => x.LookupId == TL.DevStatus).SingleOrDefault().LookupName,
                    TesterStatus = TL.TesterStatus,
                    TesterStausName = testerStatusName,
                    AssignedTo = TL.AssignedTo,
                    AllottedHrs = TL.AllottedHrs,
                    DueDate = TL.DueDate,
                    StartedDate = TL.StartedDate,
                    ClosedDate = TL.ClosedDate,
                    AssignedByName = assignToName,
                    RejectReason = TL.RejectReason,
                    RequestFlag = TL.RequestFlag,
                    RequestReason = TL.RequestReason
                });


            }

            return PartialView(taskLists);
        }

        private void getNotification(int memberId)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            int roleId = ViewBag.RoleId;

            var myInClause = new string[] { "Init_Status", "Dev_Status", "Test_Status" };
            var lookupCache = db.Lookups.Where(x => myInClause.Contains(x.LookupType)).ToList();

            ///
            Hashtable lookupIds = new Hashtable();
            lookupIds.Add("I_P", lookupCache.Where(x => x.LookupType == "Init_Status" && x.LookupName == "Pending").SingleOrDefault().LookupId);
            lookupIds.Add("I_A", lookupCache.Where(x => x.LookupType == "Init_Status" && x.LookupName == "Assigned").SingleOrDefault().LookupId);
            lookupIds.Add("D_W", lookupCache.Where(x => x.LookupType == "Dev_Status" && x.LookupName == "Working").SingleOrDefault().LookupId);
            lookupIds.Add("D_R", lookupCache.Where(x => x.LookupType == "Dev_Status" && x.LookupName == "Rejected").SingleOrDefault().LookupId);
            lookupIds.Add("D_C", lookupCache.Where(x => x.LookupType == "Dev_Status" && x.LookupName == "Completed").SingleOrDefault().LookupId);
            lookupIds.Add("T_R", lookupCache.Where(x => x.LookupType == "Test_Status" && x.LookupName == "Re-Open").SingleOrDefault().LookupId);
            lookupIds.Add("T_C", lookupCache.Where(x => x.LookupType == "Test_Status" && x.LookupName == "Closed").SingleOrDefault().LookupId);
            lookupIds.Add("D_P", lookupCache.Where(x => x.LookupType == "Dev_Status" && x.LookupName == "Pending").SingleOrDefault().LookupId);
            lookupIds.Add("I_O", lookupCache.Where(x => x.LookupType == "Init_Status" && x.LookupName == "Open").SingleOrDefault().LookupId);
            lookupIds.Add("I_C", lookupCache.Where(x => x.LookupType == "Init_Status" && x.LookupName == "Closed").SingleOrDefault().LookupId);
            ///


            var initOptions = new int[] { Convert.ToInt32(lookupIds["I_P"].ToString()), Convert.ToInt32(lookupIds["I_A"].ToString()), Convert.ToInt32(lookupIds["I_O"].ToString()), Convert.ToInt32(lookupIds["I_C"].ToString()) };
            var devOptions = new int[] { Convert.ToInt32(lookupIds["D_W"].ToString()), Convert.ToInt32(lookupIds["D_R"].ToString()), Convert.ToInt32(lookupIds["D_C"].ToString()), Convert.ToInt32(lookupIds["D_P"].ToString()) };
            var testerOptions = new int[] { Convert.ToInt32(lookupIds["T_R"].ToString()), Convert.ToInt32(lookupIds["T_C"].ToString()) };

            if (memberId > 0)
            {
                if (roleId != 4)
                {
                    ///DEVELOPER
                    var requiredTaskCache = db.Tasks.Where(
                       x => (initOptions.Contains(x.InitStatus) ||
                           devOptions.Contains(x.DevStatus) ||
                           testerOptions.Contains(x.TesterStatus) ||
                           x.RequestFlag == true) &&
                           x.AssignedTo == memberId
                       ).ToList();

                    ///VIEW BAG LIST
                    ViewBag.PendingList = db.Tasks.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_P"].ToString())).Count();

                    ViewBag.AssignedList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == false && x.AssignedTo == memberId).Count();
                    ViewBag.WorkingList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_W"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.AssignedTo == memberId).Count();
                    ViewBag.RejectedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.AssignedTo == memberId).Count();
                    ViewBag.ReOpenList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.AssignedTo == memberId).Count();
                    ViewBag.CloseList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_C"].ToString()) && x.AssignedTo == memberId).Count();
                    ViewBag.RequestedList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == true && x.AssignedTo == memberId).Count();
                    ViewBag.CompletedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.AssignedTo == memberId).Count();
                    ViewBag.MyList = 0;
                }
                else
                {
                    ///TESTER
                    var requiredTaskCache = db.Tasks.Where(
                       x => (initOptions.Contains(x.InitStatus) ||
                           devOptions.Contains(x.DevStatus) ||
                           testerOptions.Contains(x.TesterStatus) ||
                           x.RequestFlag == true) ||
                           x.PostedBy == memberId
                       ).ToList();

                    ///VIEW BAG LIST
                    ViewBag.PendingList = db.Tasks.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_P"].ToString())).Count();

                    ViewBag.MyList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == false && x.PostedBy == memberId).Count();
                    ViewBag.WorkingList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_W"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.PostedBy == memberId).Count();
                    ViewBag.RejectedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString())).Count();
                    ViewBag.ReOpenList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.PostedBy == memberId).Count();
                    ViewBag.CloseList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_C"].ToString()) && x.PostedBy == memberId).Count();
                    ViewBag.RequestedList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == true && x.PostedBy == memberId).Count();
                    ViewBag.CompletedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.PostedBy == memberId).Count();
                    ViewBag.AssignedList = 0;
                }


               
                return;
            }
            else
            {


                /// LISTING ALL TASKS
                var requiredTaskCache = db.Tasks.Where(
                    x => initOptions.Contains(x.InitStatus) ||
                        devOptions.Contains(x.DevStatus) ||
                        testerOptions.Contains(x.TesterStatus) ||
                        x.RequestFlag == true
                    ).ToList();


                ///VIEW BAG LIST
                ViewBag.PendingList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_P"].ToString())).Count();

                ViewBag.AssignedList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == false).Count();
                ViewBag.WorkingList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_W"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString())).Count();
                ViewBag.RejectedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString())).Count();
                ViewBag.ReOpenList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_R"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_A"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString())).Count();
                ViewBag.CloseList = requiredTaskCache.Where(x => x.TesterStatus == Convert.ToInt32(lookupIds["T_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_C"].ToString())).Count();
                ViewBag.RequestedList = requiredTaskCache.Where(x => x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString()) && x.DevStatus == Convert.ToInt32(lookupIds["D_P"].ToString()) && x.RequestFlag == true).Count();
                ViewBag.CompletedList = requiredTaskCache.Where(x => x.DevStatus == Convert.ToInt32(lookupIds["D_C"].ToString()) && x.InitStatus == Convert.ToInt32(lookupIds["I_O"].ToString())).Count();
                ViewBag.MyList = 0;

                return;
            }


        }


        public ActionResult DevAccepted(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
            taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

            if (taskToUpdate.AllottedHrs > 0)
            {
                taskToUpdate.DueDate = DateTime.Now.AddHours(taskToUpdate.AllottedHrs);
            }
            taskToUpdate.StartedDate = DateTime.Now;

            db.SaveChanges();

            saveHistory(id, "DevAccepted", "With due date " + taskToUpdate.DueDate.ToString());

            return RedirectToAction("Create");
        }
        public ActionResult DevCompleted(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Open" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
            taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Completed" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

            taskToUpdate.ClosedDate = DateTime.Now;

            db.SaveChanges();

            saveHistory(id, "DevCompleted", "");

            return RedirectToAction("Create");
        }
        [Authorize]
        public ActionResult _Notification_Partial()
        {
            int userId = Convert.ToInt32(Session["UserId"].ToString());
            var db = new TaskAssign.Models.DatabaseContext();

            /// Cached data from DB
            var lookups = db.Lookups.Where(x => x.LookupType == "Init_Status" || x.LookupType == "Dev_Status").ToList();
            var myTasks = db.Tasks.Where(x => x.AssignedTo == userId).ToList();
            ///

            int assignedId = lookups.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;
            int aceeptedId = lookups.Where(x => x.LookupName == "Working" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
            int rejectedId = lookups.Where(x => x.LookupName == "Rejected" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;

            ViewBag.Assigned = myTasks.Where(x => x.InitStatus == assignedId).Count();
            ViewBag.Accepted = myTasks.Where(x => x.DevStatus == aceeptedId).Count();
            ViewBag.Rejected = myTasks.Where(x => x.DevStatus == rejectedId).Count();

            return PartialView();
        }

        [Authorize]
        public ActionResult _RequestTime(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var request = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();
            TaskAssign.Models.TaskList TL = new TaskAssign.Models.TaskList();

            TL.TaskId = id;
            TL.RequestReason = request.RequestReason;
            TL.RequestFlag = request.RequestFlag;
            TL.AssignedTo = request.AssignedTo;
            TL.RejectReason = "";


            ViewBag.UserId = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.Role = Convert.ToInt32(Session["Role"].ToString());
            return PartialView(TL);
        }

        [HttpPost]
        [Authorize]
        public ActionResult _RequestTime(int id, TaskAssign.Models.TaskList TL)
        {
            var db = new TaskAssign.Models.DatabaseContext();

            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            taskToUpdate.RequestFlag = true;
            taskToUpdate.RequestHours = TL.RequestHours;

            string reasonMsg = "<div class='reqHeader'>" +
                    "Requested On: " + DateTime.Now.ToString() + "<br/>" +
                    "Requested Hour: " + TL.RequestHours + "<br />" +
                    "-----------------------------<br />" +
                    "Reason >> <br />" +
                    TL.RequestReason +
                    "<br />-----------------------------<br />" +
                    "</div>";



            taskToUpdate.RequestReason = reasonMsg + taskToUpdate.RequestReason;
            taskToUpdate.RequestFlag = true;

            db.SaveChanges();

            return RedirectToAction("Create");
        }

        [HttpPost]
        [Authorize]
        public ActionResult respondRequest(int id, string reqType, TaskAssign.Models.TaskList TL)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();

            string message = "";
            if (reqType == "Accept")
            {
                taskToUpdate.RequestFlag = false;

                string reasonMsg = "<div class='replyHeader'>" +
               "Accepted<br />" +
               "<hr style='border:1px solid gray;'/>" +
              "</div>";
                taskToUpdate.RequestReason = reasonMsg + taskToUpdate.RequestReason;

                taskToUpdate.AllottedHrs = taskToUpdate.AllottedHrs + taskToUpdate.RequestHours;
                taskToUpdate.DueDate = taskToUpdate.StartedDate.AddHours(taskToUpdate.AllottedHrs);

                taskToUpdate.RequestFlag = false;

                message = "for " + taskToUpdate.AllottedHrs + " Hours with due date on " + taskToUpdate.DueDate.ToString();

            }
            else if (reqType == "Reject")
            {
                taskToUpdate.RequestFlag = false;
                string reasonMsg = "<div class='replyHeader'>" +
                "Rejected >> <br />" +
                TL.RequestReason +
                "<hr style='border:1px solid gray;'/>" +
               "</div>";
                taskToUpdate.RequestReason = "<div style='color:gray !important;'>" + taskToUpdate.RequestReason + "</div>" + reasonMsg;
                message = TL.RequestReason;

                taskToUpdate.RequestFlag = false;
            }

            db.SaveChanges();

            saveHistory(id, "Request" + reqType, message);

            return RedirectToAction("Create");
        }

        [Authorize]
        public ActionResult _ReOpen(int id)
        {
            var db = new TaskAssign.Models.DatabaseContext();
            var request = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();
            TaskAssign.Models.TaskList TL = new TaskAssign.Models.TaskList();

            TL.TaskId = id;
            TL.RejectReason = request.RejectReason;


            ViewBag.UserId = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.Role = Convert.ToInt32(Session["Role"].ToString());
            return PartialView(TL);
        }

        [HttpPost]
        [Authorize]
        public ActionResult _ReOpen(int id, TaskAssign.Models.TaskList TL)
        {
            var db = new TaskAssign.Models.DatabaseContext();

            var taskToUpdate = db.Tasks.Where(x => x.TaskId == id).SingleOrDefault();
            int userId = Convert.ToInt32(Session["UserId"].ToString());
            string reopenedBy = db.Members.Where(x => x.UserId == userId).SingleOrDefault().Name;

            string reasonMsg = "<div class='reqHeader'>" +
                    "Re-Opened On: " + DateTime.Now.ToString() + "<br/>" +
                    "Re-Opened by: " + reopenedBy + "<br />" +
                    "-----------------------------<br />" +
                    TL.RejectReason +
                    "<hr style='border:1px solid gray;'/>" +
                    "</div>";

            taskToUpdate.TesterStatus = db.Lookups.Where(x => x.LookupName == "Re-Open" && x.LookupType == "Test_Status").SingleOrDefault().LookupId;
            taskToUpdate.DevStatus = db.Lookups.Where(x => x.LookupName == "Pending" && x.LookupType == "Dev_Status").SingleOrDefault().LookupId;
            taskToUpdate.InitStatus = db.Lookups.Where(x => x.LookupName == "Assigned" && x.LookupType == "Init_Status").SingleOrDefault().LookupId;

            taskToUpdate.RejectReason = "<div style='color:gray !important;'>" + taskToUpdate.RejectReason + "</div>" + reasonMsg;
            db.SaveChanges();

            saveHistory(id, "Reopened", TL.RejectReason);

            return RedirectToAction("Create");
        }
        public ActionResult _showHistory(int id)
        {

            var db = new TaskAssign.Models.DatabaseContext();

            TaskAssign.Models.History HL = new Models.History();
            var historyToShow = db.Histories.Where(x => x.TaskId == id).SingleOrDefault();

            if (historyToShow != null)
            {
                HL.TaskId = historyToShow.TaskId;
                HL.Messages = historyToShow.Messages;
            }
            else
            {
                HL = null;
            }

            return PartialView(HL);
        }



        ///Private function to save history
        private int saveHistory(int taskId, string type, string message)
        {
            int success = 0;

            try
            {


                if (type == "Posted")
                {
                    var db = new TaskAssign.Models.DatabaseContext();
                    TaskAssign.Models.History HL = new Models.History();

                    HL.TaskId = taskId;

                    string formatMessage = "";


                    formatMessage = "<div>" +
                        "User: " + Session["Name"].ToString() + "[#" + Session["UserId"].ToString() + "]<br/>" +
                        "Date: " + DateTime.Now.ToString() + "<br/>" +
                        "--------------------------------------<br/>" +
                        "Task[#" + HL.TaskId + "] was posted<br/>" +
                        "<hr/></div>";

                    HL.Messages = formatMessage;

                    db.Histories.Add(HL);
                    db.SaveChanges();

                }
                else
                {
                    var db = new TaskAssign.Models.DatabaseContext();
                    var historyToEdit = db.Histories.Where(x => x.TaskId == taskId).SingleOrDefault();

                    string formatMessage = "<div>" +
                       "User: " + Session["Name"].ToString() + "[#" + Session["UserId"].ToString() + "]<br/>" +
                       "Date: " + DateTime.Now.ToString() + "<br/>" +
                       "--------------------------------------<br/>";

                    switch (type)
                    {
                        case "Accepted":
                            formatMessage += "Task was Accepted<br/>";
                            break;
                        case "Rejected":
                            formatMessage += "Task was Rejected with <span style='cursor:pointer;color:red;' onclick='openReason(this)' tile='" + message + "' style='color:red;'>Reason</span><br/>" +
                                         "<div class='reasonDiv' style='display: none; padding: 1px; border-left: 1px solid gray;margin-left:5px;'>" +
                                        message +
                                        "</div>";
                            break;
                        case "Requested":
                            formatMessage += "User requested for " + message + " Hours<br/>";
                            break;
                        case "Request-Accept":
                            formatMessage += "User request was accepted<br/>";
                            break;
                        case "Request-Reject":
                            formatMessage += "Request was Rejected with <span style='cursor:pointer;color:red;' onclick='openReason(this)' tile='" + message + "' style='color:red;'>Reason</span><br/>" +
                                        "<div class='reasonDiv' style='display: none; padding: 1px; border-left: 1px solid gray;margin-left:5px;'>" +
                                       message +
                                       "</div>";
                            break;
                        case "Assigned":
                            formatMessage += "Task was assigned to " + message + "<br/>";
                            break;
                        case "Assigned-s":
                            formatMessage += "Task was assigned in <b>Strict</b> Mode to " + message + "<br/>";
                            break;
                        case "Edited":
                            formatMessage += "User edited the task<br/>";
                            break;
                        case "Reopened":
                            formatMessage += "Task was reopened with <span style='cursor:pointer;color:red;' onclick='openReason(this)' tile='" + message + "' style='color:red;'>Reason</span><br/>" +
                                        "<div class='reasonDiv' style='display: none; padding: 1px; border-left: 1px solid gray;margin-left:5px;'>" +
                                       message +
                                       "</div>";
                            break;
                        case "DevAccepted":
                            formatMessage += "User accepted the task " + message + "<br/>";
                            break;
                        case "DevCompleted":
                            formatMessage += "User Completed the task<br/>";
                            break;


                        default:
                            break;
                    }

                    formatMessage += "<hr/></div>";

                    historyToEdit.Messages = formatMessage + "<div style='color:gray !important;'>" + historyToEdit.Messages + "</div>";
                    db.SaveChanges();
                }

                success = 1;

            }
            catch
            {

            }

            return success;

        }
    }
}
