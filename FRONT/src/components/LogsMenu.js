import React, { useState } from 'react';
import { getSystemLog, getUserLogsList, getUserLog } from '../handlers/api';

const LogsMenu = ({ onBack }) => {
  const [view, setView] = useState('main'); // main, system, users, userLog
  const [systemLog, setSystemLog] = useState('');
  const [usersList, setUsersList] = useState([]);
  const [userLog, setUserLog] = useState('');
  const [selectedUser, setSelectedUser] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSystemLog = async () => {
    setLoading(true);
    setError('');
    try {
      const data = await getSystemLog();
      setSystemLog(data.content);
      setView('system');
    } catch (err) {
      setError('Ошибка загрузки системного лога');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleUsersLog = async () => {
    setLoading(true);
    setError('');
    try {
      console.log('Загрузка списка логов пользователей...');
      const data = await getUserLogsList();
      console.log('Получены данные:', data);
      setUsersList(data);
      setView('users');
    } catch (err) {
      setError(`Ошибка загрузки списка пользователей: ${err.message}`);
      console.error('Ошибка:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUserLogClick = async (fileName, userName) => {
    setLoading(true);
    setError('');
    try {
      const data = await getUserLog(fileName);
      setUserLog(data.content);
      setSelectedUser(userName);
      setView('userLog');
    } catch (err) {
      setError('Ошибка загрузки лога пользователя');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleBack = () => {
    if (view === 'userLog') {
      setView('users');
    } else if (view === 'system' || view === 'users') {
      setView('main');
    } else {
      onBack();
    }
  };

  return (
    <div className="menu-container">
      <h1 className="menu-title">Логи</h1>

      {error && <div className="error-message">{error}</div>}

      {view === 'main' && (
        <div className="menu-buttons">
          <button className="menu-btn" onClick={handleSystemLog} disabled={loading}>
            Системные логи
          </button>
          <button className="menu-btn" onClick={handleUsersLog} disabled={loading}>
            Логи пользователей
          </button>
          <button className="menu-btn secondary" onClick={onBack}>
            Назад в главное меню
          </button>
        </div>
      )}

      {view === 'system' && (
        <div>
          <div className="log-content">
            <h2>Системный лог</h2>
            <pre>{systemLog}</pre>
          </div>
          <div className="menu-buttons" style={{ marginTop: '20px' }}>
            <button className="menu-btn secondary" onClick={handleBack}>
              Назад
            </button>
          </div>
        </div>
      )}

      {view === 'users' && (
        <div>
          <h2 style={{ marginBottom: '20px', color: '#495057' }}>Выберите пользователя</h2>
          <div className="user-list">
            {usersList.length === 0 ? (
              <p style={{ textAlign: 'center', color: '#6c757d' }}>Нет логов пользователей</p>
            ) : (
              usersList.map((user) => (
                <div
                  key={user.fileName}
                  className="user-item"
                  onClick={() => handleUserLogClick(user.fileName, user.userName)}
                  style={{ cursor: 'pointer' }}
                >
                  <strong>{user.userName}</strong> (Chat ID: {user.chatId})
                </div>
              ))
            )}
          </div>
          <div className="menu-buttons" style={{ marginTop: '20px' }}>
            <button className="menu-btn secondary" onClick={handleBack}>
              Назад
            </button>
          </div>
        </div>
      )}

      {view === 'userLog' && (
        <div>
          <div className="log-content">
            <h2>Лог пользователя: {selectedUser}</h2>
            <pre>{userLog}</pre>
          </div>
          <div className="menu-buttons" style={{ marginTop: '20px' }}>
            <button className="menu-btn secondary" onClick={handleBack}>
              Назад к списку
            </button>
          </div>
        </div>
      )}

      {loading && (
        <div style={{ textAlign: 'center', padding: '20px', fontSize: '18px', color: '#667eea' }}>
          Загрузка...
        </div>
      )}
    </div>
  );
};

export default LogsMenu;
