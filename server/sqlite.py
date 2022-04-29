import sqlite3
import os.path
from time import time
import sys

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
dbPath = os.path.join(BASE_DIR, 'database.sqlite')

def connectToDB():
    return sqlite3.connect(dbPath)

### Search
# Users
def getUsers():
    users = []
    try:
        conn = connectToDB()
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()
        cur.execute('SELECT * FROM users')
        rows = cur.fetchall()

        for row in rows:
            user = {}
            user['id'] = row['id']
            user['lastLogin'] = row['lastLogin']
            users.append(user)
    
    except:
        users = []

    return users

def getUserById(userId):
    user = {}
    try:
        conn = connectToDB()
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()
        cur.execute('SELECT * FROM users WHERE id = ?', (userId,))
        row = cur.fetchone()

        user['id'] = row['id']
        user['lastLogin'] = row['lastLogin']

    except:
        user = {}

    return user

# Auction
def getAuction(filter = None):
    auctionItems = []
    try:
        conn = connectToDB
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()

        if not filter:
            cur.execute('SELECT * FROM auction')
            rows = cur.fetchall
            
            for row in rows:
                auctionItem = {}
                auctionItem['userId'] = row['uesrId']
                auctionItem['itemId'] = row['itemId']
                auctionItem['price'] = row['price']
                auctionItem['time'] = row['time']
                auctionItems.append(auctionItem)
        
        # elif filter.find('rank') > -1:
        #     # rank 필터
        #     pass
        # elif filter.find('type') > -1:
        #     # partNum 필터
        #     pass
        # else:
        #     #
        #     pass

    except:
        auctionItems = []

    return auctionItems

# Pushes
def getPushById(userId):
    needToUpdate = False
    pushes = []
    try:
        conn = connectToDB()
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()
        cur.execute('SELECT * FROM push WHERE userId = ?', (userId,))
        rows = cur.fetchall()

        for row in rows:
            push = {}
            push['userId'] = row['userId']
            push['itemId'] = row['itemId']
            # 수령기간이 끝난 아이템이 있는 경우
            # if row['time'] > !!!:
            #   needToUpdate = True
            #   continue
            push['time'] = row['time']
            pushes.append(push)

    except:
        pushes = []

    return {
        'pushes': pushes,
        'needToUpdate': needToUpdate
    }

### Update
# Users
def insertUser(user):
    insertedUser = {}
    try:
        conn = connectToDB()
        cur = conn.cursor()
        cur.execute("INSERT INTO users (id, lastLogin) VALUES (?, ?)",(user['id'], str(int(time()))))
        conn.commit()
        insertedUser = getUserById(user['id'])

    except:
        conn().rollback()
        
    finally:
        conn.close()

    return insertedUser

# Pushes
def DeleteTimeoutPushes(userId):
    pass