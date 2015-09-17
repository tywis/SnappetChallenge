using Newtonsoft.Json;
using SnappetProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SnappetProject.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Work(string SubmitDate, string GenerateReport)
        {
            if (SubmitDate == null)
                SubmitDate = "3/24/2015";
            IEnumerable<Work> workData;

            // Get the correct file path
            var dataPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
            var filePath = Path.Combine(dataPath, "App_Data\\work.json");

            DateTime submitDate = Convert.ToDateTime(SubmitDate);

            // Serialize Json to objects
            using (StreamReader file = System.IO.File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                workData = (IEnumerable<Work>)serializer.Deserialize(file, typeof(IEnumerable<Work>));
            }
            workData = RemoveAllRecordsAfterToday(workData);

            // Return only the answers for today
            IEnumerable<Work> totalRecordsByDate = workData.Where(x => x.SubmitDate == submitDate).AsEnumerable<Work>();
            if (totalRecordsByDate.Count() == 0)
            {
                ViewBag.ErrorMessageForNoRecord = "No record found for "+ submitDate + ", please choose some other date. Showing data for today";
                submitDate = Convert.ToDateTime("3/24/2015");
                totalRecordsByDate = workData.Where(x => x.SubmitDate == submitDate).AsEnumerable<Work>();
            }
            var subjectList = (from records in totalRecordsByDate
                               group records by records.Subject into subjectGroup
                               select subjectGroup);
            var cou = subjectList.Count();
            List<User> listOfUsers = new List<User>();
            List<Subject> listOfSubjects = new List<Subject>();
            foreach (var subject in subjectList)
            {
                Console.WriteLine("Key: {0}", subject.Key);
                Boolean isFirstTime = true;
                foreach (var records in subject)
                {
                    int newProgress = records.Progress;
                    int newUserId = records.UserId;
                    String subjectName = subject.Key;
                    if (isFirstTime)
                    {
                        //Add first record in User
                        User newUser = new User();
                        newUser.UserId = newUserId;
                        newUser.Progress = newProgress;
                        newUser.Subject = subjectName;
                        newUser.SubmitDate = submitDate;
                        listOfUsers.Add(newUser);

                        //Add first record in Subject
                        Subject sub = new Subject();
                        sub.SubjectName = subjectName;
                        sub.Progress = newProgress;
                        sub.SubmitDate = submitDate;

                        isFirstTime = false;
                    }
                    else
                    {
                        listOfSubjects = generateSubjectList(newProgress, submitDate, listOfSubjects, subjectName);
                        listOfUsers = generateUserList(newUserId, newProgress, submitDate, listOfUsers, subjectName);
                    }
                }

                int c = listOfUsers.Count();
            }
            ViewBag.Users = listOfUsers;
            ViewBag.Subjects = listOfSubjects;
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(subjectGraph(listOfSubjects));
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(userGraph(listOfUsers));
            return View(listOfSubjects);
        }

        public List<Subject> generateSubjectList(int newProgress, DateTime submitDate, List<Subject> listOfSubjects, string subjectName)
        {
            Boolean isNewSubject = true;
            foreach (var sub in listOfSubjects)
            {
                if (sub.SubjectName.Equals(subjectName))
                {
                    isNewSubject = false;
                    sub.Progress += newProgress;
                }
            }
            if (isNewSubject)
            {
                Subject tempSubject = new Subject();
                tempSubject.Progress = newProgress;
                tempSubject.SubjectName = subjectName;
                tempSubject.SubmitDate = submitDate;
                listOfSubjects.Add(tempSubject);
            }
            return listOfSubjects;
        }

        public List<User> generateUserList(int newUserId, int newProgress, DateTime submitDate, List<User> listOfUsers, string subjectName)
        {
            Boolean isNewUser = true;
            foreach (var user in listOfUsers)
            {
                if (user.UserId == newUserId && user.Subject.Equals(subjectName))
                {
                    isNewUser = false;
                    user.Progress += newProgress;
                }
            }
            if (isNewUser)
            {
                User tempUser = new User();
                tempUser.UserId = newUserId;
                tempUser.Progress = newProgress;
                tempUser.Subject = subjectName;
                tempUser.SubmitDate = submitDate;
                listOfUsers.Add(tempUser);
            }
            return listOfUsers;
        }
        #region
        ////Method to generate subject-wise progress report.
        //public void generateSubjectWiseReport(string SubmitDate, IEnumerable<IGrouping<string,int>> subjectList)
        //{
        //    List<User> listOfUsers = new List<User>();
        //    foreach (var subject in subjectList)
        //    {
        //        //Console.WriteLine("Key: {0}", subject.Key);
        //        //Boolean isFirstUser = true;
        //        foreach (var records in subject)
        //        {
        //            int newProgress = records.Progress;
        //            int newUserId = records.UserId;
        //            Boolean isNewUser = true;
        //            if (isFirstUser)
        //            {
        //                User newUser = new User();
        //                newUser.UserId = newUserId;
        //                newUser.Progress = newProgress;
        //                newUser.Subject = subject.Key;
        //                newUser.SubmitDate = Convert.ToDateTime(SubmitDate);
        //                listOfUsers.Add(newUser);
        //                isFirstUser = false;
        //            }
        //            else
        //            {
        //                foreach (var user in listOfUsers)
        //                {
        //                    if (user.UserId == newUserId && user.Subject.Equals(subject.Key))
        //                    {
        //                        isNewUser = false;
        //                        user.Progress += newProgress;
        //                    }
        //                }
        //                if (isNewUser)
        //                {
        //                    User tempUser = new User();
        //                    tempUser.UserId = newUserId;
        //                    tempUser.Progress = newProgress;
        //                    tempUser.Subject = subject.Key;
        //                    tempUser.SubmitDate = Convert.ToDateTime(SubmitDate);
        //                    listOfUsers.Add(tempUser);
        //                }
        //            }
        //        }

        //        int c = listOfUsers.Count();
        //    }
        //}
        #endregion

        //Method for removinf records for date after TODAY: 2015-03-24T11:30:05.130.
        public IEnumerable<Work> RemoveAllRecordsAfterToday(IEnumerable<Work> workData)
        {
            IEnumerable<Work> totalValidRecordsByDate = workData.Where(x => Convert.ToDateTime(x.SubmitDateTime) < Convert.ToDateTime("2015-03-24T11:30:05.130")).AsEnumerable<Work>();
            return totalValidRecordsByDate;
        }

        public List<DataPoint> subjectGraph(List<Subject> listOfSubjects)
        {
            List<DataPoint> subjects = new List<DataPoint>();
            foreach (var sub in listOfSubjects)
            {
                DataPoint dp = new DataPoint(sub.SubjectName, sub.Progress);
                subjects.Add(dp);

            }
            return subjects;
        }

        public List<DataPoint> userGraph(List<User> listOfUsers)
        {
            List<DataPoint> users = new List<DataPoint>();
            foreach (var user in listOfUsers)
            {
                DataPoint dp = new DataPoint((user.UserId).ToString(), user.Progress);
                users.Add(dp);

            }
            return users;
        }
    }
}
