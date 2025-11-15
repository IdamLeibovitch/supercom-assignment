import { useState, useEffect, useRef } from 'react';
import UsersTableFull from './UsersTableFull';
import UsersTableCompact from './UsersTableCompact';
import UsersTableMobile from './UsersTableMobile';
import ErrorBoundary from '../shared/ErrorBoundary';

function UsersTableResponsive({ users }) {
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
      return <UsersTableMobile users={users} />;
    } else if (width < 960) {
      return <UsersTableCompact users={users} />;
    } else {
      return <UsersTableFull users={users} />;
    }
  };

  return (
    <ErrorBoundary
      title="Table Error"
      message="Failed to render users table."
    >
      <div ref={containerRef}>{getTableComponent()}</div>
    </ErrorBoundary>
  );
}

export default UsersTableResponsive;
