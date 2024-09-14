from flask import Flask, render_template, request, abort, send_from_directory
import os

app = Flask(__name__)

# Base directory where your templates are stored
TEMPLATE_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'templates')
app.template_folder = TEMPLATE_DIR

# Directory where assets are stored
ASSET_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'assets')

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
    return render_template(template_file, params=template_vars)

# Serve assets from the /_asset/ route
@app.route('/_asset/<path:filename>')
def serve_asset(filename):
    # Send the requested file from the assets directory
    return send_from_directory(ASSET_DIR, filename)

# For local development: running the Flask app
if __name__ == '__main__':
    app.run(debug=True, port=5001)
