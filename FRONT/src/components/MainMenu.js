import React from 'react';

function MainMenu({ onNavigate }) {
  return (
    <div className="menu-container">
      <h1 className="menu-title">NotifySys Admin</h1>
      <div className="menu-buttons">
        <button className="menu-btn" onClick={() => onNavigate('regions')}>
          1. Регионы
        </button>
        <button className="menu-btn" onClick={() => onNavigate('users')}>
          2. Пользователи
        </button>
        <button className="menu-btn" onClick={() => onNavigate('sendWarning')}>
          3. Отправить предупреждение
        </button>
        <button className="menu-btn danger" onClick={() => window.close()}>
          4. Завершить работу
        </button>
      </div>
      <div className="menu-buttons" style={{ marginTop: '40px' }}>
        <button className="menu-btn" onClick={() => onNavigate('logs')}>
          Логи
        </button>
      </div>
    </div>
  );
}

export default MainMenu;
