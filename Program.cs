using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.VisualBasic;

namespace Program
{
    public enum PriorityLevel { Low, Medium, High }
    public enum Status { To_Do, In_Progress, Completed }
    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public PriorityLevel Priority { get; set; }
        public Status TaskStatus { get; set; }
        public string Assignee { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }

    class App
    {
        static HashSet<string> cmds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        static string[] cmdsorig = ["Help", "View", "Add", "Update", "Remove"];
        static string[] dateFormats = ["d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy HH:mm:ss", "dd/M/yyyy HH:mm:ss", "d/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss"];
        static int tasknum = 0;
        static Dictionary<int, Task> Tasks = new Dictionary<int, Task>(); 
        static void PrintCommands()
        {
            Console.WriteLine("Current available commands:");
            foreach (var cmd in App.cmds)
            {
                Console.WriteLine(cmd);
            }
        }

        static void ExportData()
        {
            File.WriteAllText("Data.txt", "");
            foreach (KeyValuePair<int, Task> kvp in Tasks)
            {
                var task = kvp.Value;
                File.AppendAllText("Data.txt", $"Index: {kvp.Key}\nName: {task.Name}\nDescription: {task.Description}\nStart Date: {task.StartDate}\nDue Date: {task.DueDate}\nPriority Level: {task.Priority}\nStatus: {task.TaskStatus}\nAssignee: {task.Assignee}\n\n");
            }
        }

        static void ImportData()
        {
            string data = File.ReadAllText("Data.txt");
            string[] tasks = data.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string task in tasks)
            {
                string[] lines = task.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                int indexVar = Convert.ToInt32(lines[0].Split("Index: ")[1].Trim());
                string nameVar = lines[1].Split("Name: ")[1].Trim();
                string descVar = lines[2].Split("Description: ")[1].Trim();
                string sdVar = lines[3].Split("Start Date: ")[1].Trim();
                string ddVar = lines[4].Split("Due Date: ")[1].Trim();
                DateTime sdVarx = DateTime.ParseExact(sdVar, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                DateTime ddVarx = DateTime.ParseExact(ddVar, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                string prioVar = lines[5].Split("Priority Level: ")[1].Trim();
                string statusVar = lines[6].Split("Status: ")[1].Trim();
                PriorityLevel prioVarx;
                Status statusVarx;
                Enum.TryParse(prioVar, true, out prioVarx);
                Enum.TryParse(statusVar, true, out statusVarx);
                string assigneeVar = lines[7].Split("Assignee: ")[1].Trim();

                Task t = new Task();
                t.Name = nameVar;
                t.Description = descVar;
                t.StartDate = sdVarx;
                t.DueDate = ddVarx;
                t.Priority = prioVarx;
                t.TaskStatus = statusVarx;
                t.Assignee = assigneeVar;
                Tasks.Add(indexVar, t);
                tasknum++;
            }
        }
        
        static void Main(string[] args)
        {
            foreach (var cmd in cmdsorig)
            {
                cmds.Add(cmd);
            }
            ImportData();
            PrintCommands();
            while (true)
            {
                Console.WriteLine("\nEnter a command:");
                string userInput = Console.ReadLine().ToLower();
                switch (userInput)
                {
                    case "help":
                        PrintCommands();
                        break;

                    case "view":
                        foreach (KeyValuePair<int, Task> kvp in Tasks)
                        {
                            var task = kvp.Value;
                            Console.WriteLine($"\nIndex: {kvp.Key}\nName: {task.Name}\nDescription: {task.Description}\nStart Date: {task.StartDate}\nDue Date: {task.DueDate}\nPriority Level: {task.Priority}\nStatus: {task.TaskStatus}\nAssignee: {task.Assignee}");
                        }
                        break;

                    case "add":
                        while (true)
                        {
                            Console.WriteLine("Enter the name of the task: ");
                            string nameInput = Console.ReadLine();
                            Console.WriteLine("Enter the description of the task: ");
                            string descriptionInput = Console.ReadLine();
                            Console.WriteLine("Enter the start date (format: day/month/year hour:minute:second) or leave empty for today:");
                            string startDateInput = Console.ReadLine();
                            DateTime startDateInput1;
                            if (string.IsNullOrWhiteSpace(startDateInput))
                            {
                                startDateInput1 = DateTime.Now;
                            }
                            else
                            {
                                startDateInput1 = DateTime.ParseExact(startDateInput, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            Console.WriteLine("Enter the due date (format: day/month/year hour:minute:second) or leave empty for a week ahead of today: ");
                            string dueDateInput = Console.ReadLine();
                            DateTime dueDateInput1;
                            if (string.IsNullOrWhiteSpace(dueDateInput))
                            {
                                dueDateInput1 = DateTime.Now.AddDays(7);
                            }
                            else
                            {
                                dueDateInput1 = DateTime.ParseExact(dueDateInput, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            Console.WriteLine("Enter the priority level of the task (low, medium, high): ");
                            string priorityLevelInput = Console.ReadLine();
                            PriorityLevel priorityLevelInput1;
                            Enum.TryParse(priorityLevelInput, true, out priorityLevelInput1);
                            Console.WriteLine("Enter the task assignee's name: ");
                            string assigneeInput = Console.ReadLine();

                            Task ntsk = new Task();
                            ntsk.Name = nameInput;
                            ntsk.Description = descriptionInput;
                            ntsk.StartDate = startDateInput1;
                            ntsk.DueDate = dueDateInput1;
                            ntsk.Priority = priorityLevelInput1;
                            ntsk.TaskStatus = Status.To_Do;
                            ntsk.Assignee = assigneeInput;
                            Console.WriteLine($"----- Task succesfully created -----\nIndex: {tasknum}\nName: {ntsk.Name}\nDescription: {ntsk.Description}\nStart Date: {ntsk.StartDate}\nDue Date: {ntsk.DueDate}\nPriority Level: {ntsk.Priority}\nStatus: {ntsk.TaskStatus}\nAssignee: {ntsk.Assignee}");
                            Tasks.Add(tasknum, ntsk);
                            tasknum++;
                            break;
                        }
                        ExportData();
                        break;

                    case "update":
                        Console.WriteLine("Enter the index of the task you wish to update: ");
                        int indexInput2 = Convert.ToInt32(Console.ReadLine());
                        if (Tasks.ContainsKey(indexInput2))
                        {
                            string[] validList = ["name", "description", "startdate", "duedate", "priority", "taskstatus", "assignee"];
                            Console.WriteLine($"Valid Updates: {string.Join(", ", validList)}");
                            Console.WriteLine($"Enter the fields you'd like to update in task {indexInput2} (format: field1, field2...): ");
                            string updatesInput = Console.ReadLine().ToLower();
                            string[] updatesInputList = updatesInput.Trim().Split(",");
                            Dictionary<string, object> oldValues = new Dictionary<string, object>();
                            foreach (var item in updatesInputList)
                            {
                                if (validList.Contains(item.Trim()))
                                {
                                    string capitalizedItem = char.ToUpper(item.Trim()[0]) + item.Trim().Substring(1);
                                    Console.WriteLine($"Enter new {capitalizedItem.Trim()}: ");
                                    string valueInput = Console.ReadLine();
                                    DateTime valueDateInput;
                                    PriorityLevel prioLev;
                                    Status tskStatus;
                                    var task = Tasks[indexInput2];
                                    switch (item.Trim())
                                    {
                                        case "name":
                                            oldValues.Add(capitalizedItem, task.Name);
                                            task.Name = valueInput;
                                            break;

                                        case "description":
                                            oldValues.Add(capitalizedItem, task.Description);
                                            task.Description = valueInput;
                                            break;

                                        case "startdate":
                                            oldValues.Add(capitalizedItem, task.StartDate);
                                            if (string.IsNullOrWhiteSpace(valueInput))
                                            {
                                                valueDateInput = DateTime.Now;
                                            }
                                            else
                                            {
                                                valueDateInput = DateTime.ParseExact(valueInput, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                                            }
                                            task.StartDate = valueDateInput;
                                            break;

                                        case "duedate":
                                            oldValues.Add(capitalizedItem, task.DueDate);
                                            if (string.IsNullOrWhiteSpace(valueInput))
                                            {
                                                valueDateInput = DateTime.Now.AddDays(7);
                                            }
                                            else
                                            {
                                                valueDateInput = DateTime.ParseExact(valueInput, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
                                            }
                                            task.DueDate = valueDateInput;
                                            break;

                                        case "priority":
                                            oldValues.Add(capitalizedItem, task.Priority);
                                            Enum.TryParse(valueInput, true, out prioLev);
                                            task.Priority = prioLev;
                                            break;

                                        case "taskstatus":
                                            oldValues.Add(capitalizedItem, task.TaskStatus);
                                            Enum.TryParse(valueInput, true, out tskStatus);
                                            task.TaskStatus = tskStatus;
                                            break;

                                        case "assignee":
                                            oldValues.Add(capitalizedItem, task.Assignee);
                                            task.Assignee = valueInput;
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Field {item} is invalid, skipping.");
                                }
                            }
                            Console.WriteLine($"----- Task {indexInput2} Update Report -----\nTotal Updates: {oldValues.Count}\nUpdated Fields:");
                            foreach (KeyValuePair<string, object> kvp in oldValues)
                            {
                                var task = Tasks[indexInput2];
                                object newValue = null;
                                switch (kvp.Key.ToLower())
                                {
                                    case "name":
                                        newValue = task.Name;
                                        break;

                                    case "description":
                                        newValue = task.Description;
                                        break;

                                    case "startdate":
                                        newValue = task.StartDate;
                                        break;

                                    case "duedate":
                                        newValue = task.DueDate;
                                        break;

                                    case "priority":
                                        newValue = task.Priority;
                                        break;

                                    case "taskstatus":
                                        newValue = task.TaskStatus;
                                        break;

                                    case "assignee":
                                        newValue = task.Assignee;
                                        break;
                                }
                                Console.WriteLine($"{kvp.Key}: {kvp.Value} => {newValue}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: No task with index {indexInput2} has been found.");
                        }
                        ExportData();
                        break;

                    case "remove":
                        Console.WriteLine("Enter the index of the task you wish to remove: ");
                        int indexInput = Convert.ToInt32(Console.ReadLine());
                        if (Tasks.ContainsKey(indexInput))
                        {
                            Tasks.Remove(indexInput);
                            Dictionary<int, Task> newTasks = new Dictionary<int, Task>(); // index update logic
                            foreach (KeyValuePair<int, Task> kvp in Tasks)
                            {
                                newTasks.Add((kvp.Key > indexInput) ? kvp.Key - 1 : kvp.Key, kvp.Value);
                            }
                            Tasks = newTasks;
                            tasknum--;
                            Console.WriteLine($"Task with index {indexInput} was succesfully removed.");
                        }
                        else
                        {
                            Console.WriteLine($"Error: No task with index {indexInput} has been found.");
                        }
                        ExportData();
                        break;

                    default:
                        Console.WriteLine("Invalid command, try again");
                        break;
                }
            }
        }
    }
}