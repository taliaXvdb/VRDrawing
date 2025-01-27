import tensorflow as tf
from tensorflow.keras import layers, models, Input
from tensorflow.keras.models import Model

def create_siamese_network(input_shape):
    input = Input(shape=input_shape)
    
    # Example CNN architecture for feature extraction
    x = layers.Conv2D(32, (3, 3), activation='relu')(input)
    x = layers.MaxPooling2D((2, 2))(x)
    x = layers.Flatten()(x)
    x = layers.Dense(128, activation='relu')(x)
    
    # Create the model that will share weights for both inputs
    model = Model(inputs=input, outputs=x)
    return model

def euclidean_distance(vectors):
    # Compute the Euclidean distance between two vectors
    x, y = vectors
    return tf.norm(x - y, axis=-1, keepdims=True)

# Input shape (e.g., 224x224 RGB image)
input_shape = (224, 224, 3)

# Create the Siamese network
base_network = create_siamese_network(input_shape)

# Input for the two images
input_a = Input(shape=input_shape)
input_b = Input(shape=input_shape)

# Process both inputs with the same network (shared weights)
output_a = base_network(input_a)
output_b = base_network(input_b)

# Compute the Euclidean distance between the outputs
distance = layers.Lambda(euclidean_distance)([output_a, output_b])

# Create the final model
siamese_model = Model(inputs=[input_a, input_b], outputs=distance)

# Compile the model
siamese_model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])

# Save the model in TensorFlow's SavedModel format
tf.saved_model.save(siamese_model, "siamese_model")  # Save as TensorFlow's SavedModel (folder format)


