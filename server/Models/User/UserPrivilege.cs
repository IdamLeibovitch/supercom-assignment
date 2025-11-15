namespace Backend.Models;

public enum UserPrivilege
{
    UsersRead,
    UsersWrite,
    UserPrivilegesRead,
    UserPrivilegesWrite,
    TasksRead,
    TasksCreate,
    TasksWrite,
    TasksDelete,
    AllTasksRead,
    AllTasksCreate, // create tasks for other users
    AllTasksWrite, // edit tasks for other users
    AllTasksDelete // delete other users tasks
}
