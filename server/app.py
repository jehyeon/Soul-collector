from flask import Flask, request, jsonify
import sqlite
import sys

app = Flask(__name__)

# Users
@app.route('/api/users')
def getUsers():
    return jsonify(sqlite.getUsers())

@app.route('/api/users/<userId>')
def getUser(userId):
    return jsonify(sqlite.getUserByUserId(userId))

@app.route('/api/users/create', methods=['POST'])
def createUser():
    user = request.get_json()
    # print('CHECK----', file=sys.stderr)
    # print(user, file=sys.stderr)
    # print('----------', file=sys.stderr)
    return jsonify(sqlite.insertUser(user))

# Auction
@app.route('/api/auction')
def getAuction():
    return jsonify(sqlite.getAuction())

@app.route('/api/auction/add', methods=['POST'])
def AddItemToAuction():
    data = request.get_json()
    return jsonify(sqlite.insertItemToAuction(data))

@app.route('/api/auction/delete', methods=['POST'])
def DeleteItemFromAuction():
    pass

# Push
@app.route('/api/push/<userId>')
def getPush(userId):
    return jsonify(sqlite.getPushById(userId))

@app.route('/api/push/add', methods=['POST'])
def AddPush():
    data = request.get_json()
    return jsonify(sqlite.insertPush(data))

# Manage
@app.route('/manage/push')
def push():
    return {'state': 'OK'}
    
if __name__ == '__main__':
    app.run()