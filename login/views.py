from django.shortcuts import render
import requests
from django.core.urlresolvers import reverse
from django.http import JsonResponse
import ClicknMine.secret as secret

import pickle

# Create your views here.
def login_success(request):
    access_token_prefix = "https://graph.facebook.com/v2.9/oauth/access_token?"
    app_id = secret.APP_ID 
    redirect_uri = secret.REDIRECT_URI
    client_secret = secret.CLIENT_SECRET
    code = ""
    token = ""
    if "code" in request.GET:
        code = request.GET["code"]
        access_token_url = access_token_prefix + "client_id=" + app_id +\
                       "&redirect_uri=" + redirect_uri +\
                       "&client_secret=" + client_secret +\
                       "&code=" + code

        s = requests.Session()
        r = s.get(access_token_url)
        print(access_token_url)
        save_db(r)

        return JsonResponse(r.json())

    elif "token" in request.GET:
        token = request.GET["token"]
        r = load_db()
        return JsonResponse(r.json())

def load_db():
    # read DB from pkl file
    with open("token", 'rb') as f:
        return pickle.load(f)

def save_db(database):
    # dump new db into a file
    output = open("token", 'wb')
    pickle.dump(database, output)