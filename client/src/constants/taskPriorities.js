export const TaskPriority = Object.freeze({
  LOW: 'Low',
  MEDIUM: 'Medium',
  HIGH: 'High',
});

export const TaskPriorityLabels = Object.freeze({
  [TaskPriority.LOW]: 'Low',
  [TaskPriority.MEDIUM]: 'Medium',
  [TaskPriority.HIGH]: 'High',
});

export const TaskPriorityColors = Object.freeze({
  [TaskPriority.LOW]: 'success',
  [TaskPriority.MEDIUM]: 'warning',
  [TaskPriority.HIGH]: 'error',
});
