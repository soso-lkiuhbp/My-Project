import jwt
import datetime

secret_key = 'MY_SECRET'

def create_token(username):
    expire = datetime.datetime.now(datetime.timezone.utc) + datetime.timedelta(hours=1)
    content = {
        'username':username,
        'exp':expire
    }
    token = jwt.encode(content,secret_key,algorithm='HS256')
    return token

def decode_token(token):
    try:
        content =jwt.decode(token,secret_key,algorithms=['HS256'])
        return content
    except jwt.ExpiredSignatureError:
        return 0
    except jwt.InvalidTokenError:
        return