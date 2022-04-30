import sqlite3
import os

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
dbPath = os.path.join(BASE_DIR, 'database.sqlite')

def main():
    print('Reset? (say "Yes")')
    reset = input('=> ')

    if not reset == 'Yes':
        pass
    
    try:
        os.remove(dbPath)
    except:
        pass

    conn = sqlite3.connect(dbPath)

    conn.execute('PRAGMA foreign_keys=ON')
    # Make users table
    conn.execute(
        '''
        CREATE TABLE users (
            id INTEGER PRIMARY KEY,
            userId TEXT NOT NULL,
            lastLogin INTEGER NOT NULL
        )
        '''
    )

    # Make auction table
    conn.execute(
        '''
        CREATE TABLE auction (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            userId TEXT NOT NULL,
            itemId INTEGER NOT NULL,
            price INTEGER NOT NULL,
            time INTEGER NOT NULL,
            rank TEXT NOT NULL,
            type INTEGER NOT NULL,
            FOREIGN KEY(userId) REFERENCES users (userId)
        )
        '''
    )
    # Make push table
    conn.execute(
        '''
        CREATE TABLE push (
            userId TEXT NOT NULL,
            itemId INTEGER NOT NULL,
            message TEXT,
            time INTEGER NOT NULL,
            FOREIGN KEY(userId) REFERENCES users (userId)
        )
        '''
    )

    conn.commit()
    conn.close()

if __name__ == '__main__':
    main()
