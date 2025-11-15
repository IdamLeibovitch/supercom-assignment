/**
 * @typedef {Object} UserPrivileges
 * @property {string} UsersRead
 * @property {string} UsersWrite
 * @property {string} UserPrivilegesRead
 * @property {string} UserPrivilegesWrite
 * @property {string} TasksRead
 * @property {string} AllTasksRead
 * @property {string} TasksCreate
 * @property {string} AllTasksCreate
 * @property {string} TasksWrite
 * @property {string} AllTasksWrite
 * @property {string} TasksDelete
 * @property {string} AllTasksDelete
 */

/** @type {UserPrivileges} */
export const UserPrivileges = Object.freeze({
    UsersRead: 'UsersRead',
    UsersWrite: 'UsersWrite',
    UserPrivilegesRead: 'UserPrivilegesRead',
    UserPrivilegesWrite: 'UserPrivilegesWrite',
    TasksRead: 'TasksRead',
    AllTasksRead: 'AllTasksRead',
    TasksCreate: 'TasksCreate',
    AllTasksCreate: 'AllTasksCreate',
    TasksWrite: 'TasksWrite',
    AllTasksWrite: 'AllTasksWrite',
    TasksDelete: 'TasksDelete',
    AllTasksDelete: 'AllTasksDelete'
});
