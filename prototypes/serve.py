from flask import Flask, render_template, request, abort, send_from_directory
import os
from faker import Faker
from datetime import datetime,timedelta
import random

app = Flask(__name__)
fake = Faker()

# Base directory where your templates are stored
TEMPLATE_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'templates')
app.template_folder = TEMPLATE_DIR

# Directory where assets are stored
ASSET_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'assets')

def fake_ts():
    r = random.random()
    tmax, ttype = random.choice([
        (None, 'just now'),
        (60, 's'),
        (60, 'm'),
        (12, 'h')
    ])
    if tmax is not None:
        return f'{random.randint(1, tmax)}{ttype}'
    return ttype


@app.route('/<path:template_path>')
def dynamic_render(template_path):
    # Append '.html' to the path to find the correct template
    template_file = f'{template_path}.html'

    # Check if the template exists in the template folder
    if not os.path.exists(os.path.join(app.template_folder, template_file)):
        abort(404)

    # Convert request args to a dictionary
    template_vars = {key: value for key, value in request.args.items()}

    # Render the template and pass the dictionary
    return render_template(template_file, params=template_vars, faker=fake, fake_ts=fake_ts, random=random)

# Serve assets from the /_asset/ route
@app.route('/_asset/<path:filename>')
def serve_asset(filename):
    # Send the requested file from the assets directory
    return send_from_directory(ASSET_DIR, filename)

# For local development: running the Flask app
if __name__ == '__main__':
    app.run(debug=True, port=5001)
