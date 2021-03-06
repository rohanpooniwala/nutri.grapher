import gensim
import os
import nltk
from nltk.stem import WordNetLemmatizer
from nltk.stem import PorterStemmer
from nltk.corpus import stopwords
from nltk import word_tokenize
import re
from sklearn.decomposition import PCA
import pickle

# globals
bad_patterns = [r'&.*;', r'[0-9].*']
stopwords = set(stopwords.words('english'))
lemmatizer = WordNetLemmatizer()
stemmer = PorterStemmer()
cur_dir = os.getcwd()
list_of_files = os.listdir(cur_dir)
bad_chars = ['.', '"', ',', '(', ')', '!', '?', ';', ':']


# fetching the labels
def fetch_labels():
    labels = []
    for doc in list_of_files:
        if '.txt' in doc:
            labels.append(doc)
    return labels


# # fetching the data associated to each label
# def get_training_data(labels):
# 	training_data = []

# 	for label in labels:
# 		path_of_doc = str(cur_dir) + '\\' + str(label)
# 		f = open(path_of_doc)
# 		recipe = f.read()
# 		training_data.append(recipe)
# 	print('length of training data : ', len(training_data))
# 	print('training data : ', training_data)
# 	return training_data

# # removing stop words and bad characters
# #lemmatizing and stemming words
# #removing bad strings and garbage data
# def clean_data(training_data):	
# 	for index, recipe in enumerate(training_data):
# 		filtered_recipe = []
# 		words_recipe = word_tokenize(recipe)
# 		for word in words_recipe:
# 			if word not in stopwords and word not in bad_chars:
# 				word = stemmer.stem(word)
# 				word = lemmatizer.lemmatize(word)
# 				for pattern in bad_patterns:
# 					word = re.sub(pattern, '', word)
# 				filtered_recipe.append(word)
# 		filtered_recipe = ' '.join(filtered_recipe)
# 		training_data[index] = filtered_recipe
# 	return training_data

# #data generator to throw a gensim object one at a time
# class LabeledLineSentence(object):
# 	def __init__(self, training_data, labels):
# 		self.labels_list = labels
# 		self.doc_list = training_data

# 	def __iter__(self):
# 		for index, doc in enumerate(self.doc_list):
# 			yield gensim.models.doc2vec.LabeledSentence(doc,[self.labels_list[index]])

# #############################################################
# ##########################TRAINING PHASE#####################
# #############################################################

# def train_model(training_data, labels):
# 	res = LabeledLineSentence(training_data, labels)
# 	model = gensim.models.Doc2Vec(size=20, window = 10, min_count=0, alpha=0.025, min_alpha=0.025, workers = 2)
# 	model.build_vocab(res)

# 	#training of model
# 	for epoch in range(5000):
# 		print('iteration'+str(epoch+1))
# 		model.train(res, total_examples=model.corpus_count, epochs=model.iter)
# 		model.alpha -= 0.002
# 		model.min_alpha = model.alpha
# 		# model.train(res, total_examples=model.corpus_count, epochs=model.iter)

# 	# saving the created model
# 	model.save('./server_assets/doc2vec.model')
# 	print('successfully saved model')

# labels = fetch_labels()
# training_data = get_training_data(labels)
# training_data = clean_data(training_data)
# train_model(training_data, labels)

###############################################################
#################TESTING PHASE#################################
###############################################################

def clean_test_data(test_data):
    filtered_recipe = []
    words_recipe = word_tokenize(test_data)
    for word in words_recipe:
        if word not in stopwords and word not in bad_chars:
            word = stemmer.stem(word)
            word = lemmatizer.lemmatize(word)
            for pattern in bad_patterns:
                word = re.sub(pattern, '', word)
            filtered_recipe.append(word)
    filtered_recipe = ' '.join(filtered_recipe)
    return filtered_recipe


# def get_main_doc_vec():
# 	main_doc_vector = []
# 	labels = fetch_labels()
# 	d2v_model = gensim.models.doc2vec.Doc2Vec.load('./server_assets/doc2vec.model')
# 	for label in labels:
# 		f = open(label, 'r')
# 		data = f.read()
# 		clean_data = clean_test_data(data)
# 		f.close()
# 		f = open(label, 'w')
# 		f.write(clean_data)
# 		f.close()
# 		vec_of_doc = d2v_model.infer_vector(word_tokenize(clean_data), alpha=0.1, min_alpha=0.0001, steps=5)
# 		main_doc_vector.append(vec_of_doc)
# 	pickle.dump(main_doc_vector, open('document_vectors.p', 'wb'))

# get_main_doc_vec()

# def get_reduced_vectors():
# 	main_doc_vector = pickle.load(open('document_vectors.p', 'rb'))
# 	pca = PCA(n_components = 3)
# 	pca.fit(main_doc_vector)
# 	reduced_doc_vectors = pca.transform(main_doc_vector)
# 	pickle.dump(reduced_doc_vectors, open('reduced_doc_vectors.p', 'wb'))
# 	pickle.dump(pca, open('./server_assets/pca_model.p', 'wb'))

# get_reduced_vectors()

def fetch_coor(test_data):
    pca_model = pickle.load(open('./server_assets/pca_model.p', 'rb'))
    test_data = clean_test_data(test_data)
    d2v_model = gensim.models.doc2vec.Doc2Vec.load('./server_assets/doc2vec.model')
    vec_of_doc = d2v_model.infer_vector(word_tokenize(test_data), alpha=0.1, min_alpha=0.0001, steps=5)
    reduced_doc_vec = pca_model.transform([vec_of_doc])
    return reduced_doc_vec[0]


if __name__ == "__main__":
    # test = ['Cheddar Cheese Cookies.txt', 'Cheddar Cheese Panini.txt', 'Cheddar Cheese Sauce.txt', 'Cheddar Potato
	# Bake.txt', 'Cheddar Potato and Gravy Bake.txt']
    test = ['Chicken Gravy (Easy!).txt', 'Chicken Gravy.txt', 'Chicken in Salsa.txt', 'Chicken Liver Appetizers.txt',
            'Chicken Livers with Bacon.txt']
    for item in test:
        vec = fetch_coor(item)
        print(item, vec)


