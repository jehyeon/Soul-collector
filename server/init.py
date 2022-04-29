import sqlite3
import os

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
dbPath = os.path.join(BASE_DIR, 'database.sqlite')

def main():
    print('Reset? (say "Yes")')
    reset = input('=> ')

    if not reset == 'Yes':
        pass
    
    os.remove(dbPath)

    conn = sqlite3.connect(dbPath)
    
    # Make users table
    conn.execute(
        '''
        CREATE TABLE users (
            id TEXT,
            lastLogin TEXT,
            PRIMARY KEY(id)
        )
        '''
    )

    # Make auction table
    conn.execute(
        '''
        CREATE TABLE auction (
            userId TEXT,
            itemId INTEGER,
            price INTEGER,
            time TEXT,
            rank INTEGER,
            type INTEGER,
            FOREIGN KEY (userId) REFERENCES users (id)
        )
        '''
    )
    # Make push table
    conn.execute(
        '''
        CREATE TABLE push (
            userId TEXT,
            itemId INTEGER,
            time TEXT,
            FOREIGN KEY (userId) REFERENCES users (id)
        )
        '''
    )

    conn.commit()
    conn.close()

if __name__ == '__main__':
    main()
