import { useState, useEffect, useRef } from 'react';
import TasksTableFull from './TasksTableFull';
import TasksTableCompact from './TasksTableCompact';
import TasksTableMobile from './TasksTableMobile';

function TasksTableResponsive({ tasks, sortBy, ascending, onSortChange, isLoading = false }) {
  const [width, setWidth] = useState(0);
  const containerRef = useRef(null);

  useEffect(() => {
    if (!containerRef.current) return;

    const resizeObserver = new ResizeObserver((entries) => {
      for (const entry of entries) {
        setWidth(entry.contentRect.width);
      }
    });

    resizeObserver.observe(containerRef.current);

    return () => resizeObserver.disconnect();
  }, []);

  const getTableComponent = () => {
    if (width < 600) {
      return <TasksTableMobile tasks={tasks} isLoading={isLoading} />;
    } else if (width < 960) {
      return <TasksTableCompact tasks={tasks} sortBy={sortBy} ascending={ascending} onSortChange={onSortChange} isLoading={isLoading} />;
    } else {
      return <TasksTableFull tasks={tasks} sortBy={sortBy} ascending={ascending} onSortChange={onSortChange} isLoading={isLoading} />;
    }
  };

  return <div ref={containerRef}>{getTableComponent()}</div>;
}

export default TasksTableResponsive;
