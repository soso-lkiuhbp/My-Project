import smtplib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

def sendEmail(recv_email,content):
    smtp_host='smtp.163.com'
    username='15812471203@163.com'
    password='QWMenjRGLAp2DPxu'
    sender_email='15812471203@163.com'
    subject='验证码'
    msg=MIMEMultipart()
    msg['From']=sender_email
    msg['To']=recv_email
    msg['Subject']=subject
    msg.attach(MIMEText(content,'plain'))
    try:
        smtp_port=465
        server=smtplib.SMTP_SSL(smtp_host,smtp_port)
        server.login(username,password)
        server.sendmail(sender_email,recv_email,msg.as_string())
        server.quit()
        return 1
    except Exception as e:
        print(e)
        return -1
