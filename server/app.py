from flask import Flask

app = Flask(__name__)

@app.route('/api/users')
def users():
    return {'state': 'OK'}

@app.route('/manage/push')
def push():
    return {'state': 'OK'}
    
if __name__ == '__main__':
    app.run()